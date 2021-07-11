using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVersions
{
    public const string GameName = "战争天下";
    public const int Versions = 1;
    public const string VersionsStr = "0.01";
    public const int SaveHeader = Versions;

    public static int currLoadVersions = SaveHeader;

    public const bool Debug = true;

    static string[] oldVersionsStr =
    {
        "0.00",     // 0
        "0.01",     // 1
    };
    public static string GetOldVersionsStr(int versions)
    {
        return oldVersionsStr[versions];
    }
}
