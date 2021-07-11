using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    Line data;

    public void OnEnable()
    {
        //data = (Line)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //设置界面垂直布局
        GUILayout.BeginVertical();

        //空一行
        GUILayout.Space(0);

        //data.key = (GameObject)EditorGUILayout.ObjectField("Defualt Cube:", data.key, typeof(GameObject), true);
        //data.key = EditorGUILayout.TextField("offset_first:", data.offset_first);
        //data.offset_first = EditorGUILayout.Vector2Field("offset_first:", data.offset_first);
        //data.offset_second = EditorGUILayout.Vector2Field("offset_second:", data.offset_second);
    }
}
