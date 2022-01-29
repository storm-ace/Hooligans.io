using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Drawing
{
    public class FlagData : ScriptableObject
    {
        public List<Color> Colors = new List<Color>();
        public string saveUrl;
        public Texture flag;

        public bool SaveData(string name)
        {
            try
            {
                EditorUtility.SetDirty(this);
                var oldUrl = saveUrl;
                var oldImageUrl = oldUrl.Substring(0, oldUrl.IndexOf(".asset")) + ".png";
            
                saveUrl = $"Assets/Save Files/Saved Flags/{name}.asset";

                AssetDatabase.RenameAsset(oldUrl, name);
                AssetDatabase.RenameAsset(oldImageUrl, name);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}