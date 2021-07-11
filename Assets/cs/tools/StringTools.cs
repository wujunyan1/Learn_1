using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class StringTools
{
    public static int TextStrLength(Text text)
    {
        Font font = text.font;

        int len = 0;
        string str = text.text;
        font.RequestCharactersInTexture(str);
        for (int i = 0; i < str.Length; i++)
        {
            CharacterInfo ch;
            font.GetCharacterInfo(str[i], out ch);
            len += ch.advance;
        }
        return len;
    }
}
