using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class BuildAssets : MonoBehaviour
{
    //创建编辑器菜单目录    
    [MenuItem("Example/Build Asset Bundles")]
    static void BuildABs()
    {
        //将这些资源包放在一个名为ABs的目录下        
        string assetBundleDirectory = "Assets/ABs";
        //如果目录不存在，就创建一个目录        
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        //刷新
        AssetDatabase.Refresh();
    }
}

