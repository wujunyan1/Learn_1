sampler2D _HexCellData;
sampler2D _campColor;
float4 _HexCellData_TexelSize;

// 这个data x 表示变暗还是变亮
// y 表示可见不可见
float4 FilterCellData(float4 data) {
	#if defined(HEX_MAP_EDIT_MODE)
		data.xy = 1;
	#endif
		return data;
}

float4 GetCellData(appdata_full v, int index) {
	float2 uv;
	uv.x = (v.texcoord2[index] + 0.5) * _HexCellData_TexelSize.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
	float4 data = tex2Dlod(_HexCellData, float4(uv, 0, 0));
	data.w *= 255;
	return FilterCellData(data);
}

float4 GetCellData(float2 cellDataCoordinates) {
	/*float2 uv;
	uv.x = cellDataCoordinates.x;
	uv.y = cellDataCoordinates.y;*/

	float2 uv = cellDataCoordinates + 0.5;
	uv.x *= _HexCellData_TexelSize.x;
	uv.y *= _HexCellData_TexelSize.y;

	return FilterCellData(tex2Dlod(_HexCellData, float4(uv, 0, 0)));
}

float4 GetCellCampData(appdata_full v, int index) {
	float2 uv;
	uv.x = (v.texcoord2[index] + 0.5) * _HexCellData_TexelSize.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * _HexCellData_TexelSize.y;


	float4 data = tex2Dlod(_campColor, float4(uv, 0, 0));
	float3 pos = mul(unity_ObjectToWorld, v.vertex);
	pos.y = 0;
	float3 gpos = float3(0, 0, 0);

	float x = (v.texcoord2[index] + 0.5) - (row * _HexCellData_TexelSize.z);
	float y = row + 0.5;

	gpos.x = (x + y * 0.5f - floor(y * 0.5f) - 0.75f) * OUTER_RADIUS * OUTER_TO_INNER * 2;
	//gpos.x = pos.x;
	gpos.z = (y * (OUTER_RADIUS * 1.5f)) - (OUTER_RADIUS * 0.75f);

	gpos = gpos - pos;
	float dis = dot(gpos, gpos);

	float visibility = max(0, dis - 36);
	visibility = min(1, visibility / 36);

	// visibility = dis / 100;

	data.w *= visibility;  // dis / 0.1; // 1 - n;
	return data;
}


float4 Muitiply(float4 _color, float4 _color2){
	fixed r = _color.r * _color2.r;
	fixed g = _color.g * _color2.g;
	fixed b = _color.b * _color2.b;
	return float4(r, g, b, 1.0f);
}

float3 Overlay(float4 _color, float4 _color2) {
	float remainA1 = 1.0 - _color.a;

	float3 r = _color.rgb * _color.a;
	float3 viewCol = remainA1 * _color2.rgb;

	return r + viewCol;
}