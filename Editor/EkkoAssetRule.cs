using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace EkkoEditor
{
    public class MyTools
    {
        public static string GetParentPath(string path)
        {
            var str = path.Split("/");
            path = path.Replace(str[str.Length - 1], "");
            return path;
        }
    }
    public class MyEditor : AssetPostprocessor 
    {

        // Texture导入之前调用，针对Texture进行设置
        public void OnPreprocessTexture()
        {
            TextureImporter impor = this.assetImporter as TextureImporter;
            var parentPath = MyTools.GetParentPath(this.assetPath);
            var rule = AssetDatabase.LoadAssetAtPath<EkkoAssetRule>(parentPath + "图片资源导入配置.asset");

            if (rule)
            {
                Debug.Log("纹理资源: " + this.assetPath + " 使用了自定义的配置");
                impor.textureType = rule.textureType;
                impor.sRGBTexture = rule.sRGB;
                impor.isReadable = rule.readAble;
            }
        }
        
        // 所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
        public static void OnPostprocessAllAssets(string[]importedAssets,string[] deletedAssets,string[] movedAssets,string[]movedFromAssetPaths)
        {
            Debug.Log ("======================资源发生变化=========================");
            foreach (string str in importedAssets)
            {
                Debug.Log("重新导入资源: " + str);
            }
            foreach (string str in deletedAssets)
            {
                Debug.Log("删除资源: " + str);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            }
        }
    }
    [CreateAssetMenu(menuName = "EKKO/图片资源导入配置", fileName = "图片资源导入配置")]
    
    [Serializable]
    public class EkkoAssetRule : ScriptableObject
    {
        [Header("图片导入设置")]
        [Space(10)]
        public TextureImporterType textureType = TextureImporterType.NormalMap;
        public bool sRGB = true;
        public bool readAble = true;
    }
    
    [CustomEditor(typeof(EkkoAssetRule))]
    internal class EkkoAssetRuleEditor : Editor
    {
        private EkkoAssetRule m_Target;

        private void OnEnable()
        {
            m_Target = (EkkoAssetRule) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("应用"))
            {
                var path = AssetDatabase.GetAssetPath(m_Target);
                var dirs = Directory.GetFiles(MyTools.GetParentPath(path));
                
                foreach (var dir in dirs)
                {
                    if (dir.IndexOf(".meta") < 0 && dir.IndexOf(".cs") < 0 && dir.IndexOf(".asset") < 0)
                    {
                        Debug.Log(dir);
                        AssetDatabase.ImportAsset(dir);
                    }
                }
                // Debug.Log("target: " + m_Target);
                //
                // var path = AssetDatabase.GetAssetPath(m_Target);
                // Debug.Log("path1: " + path);
                //
                //
                // path = path.Replace("/图片资源导入配置.asset", "");
                // Debug.Log("path2: " + path);
                //
                // DirectoryInfo direction = new DirectoryInfo(path);
                // Debug.Log("direction: " + direction);
                //
                // FileInfo[] files = direction.GetFiles("*");
                //
                // foreach (var file in files)
                // {
                //     if (file.Extension == ".meta" || file.Extension == ".cs" || file.Name == "图片资源导入配置.asset")
                //     {
                //         continue;
                //     }
                //     AssetDatabase.ImportAsset(path + "/" +file.Name);
                // }
                //
                //
                // var dirs = Directory.GetDirectories(path);
                //
                // foreach (var dir in dirs)
                // {
                //     AssetDatabase.ImportAsset(dir, ImportAssetOptions.ImportRecursive);
                // }
            }
        }
    }
    

    
}