using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Drawing;
using Saving;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class SaveAndStoreManager : MonoBehaviour
{
    public Camera screenShotCamera;

    public GameObject savePanel;

    public Transform savedFlagsGroup;

    public GameObject flagPrefab;
    
    public InputField saveNameText;

    [SerializeField] private GridManager gridManager;

    private Dictionary<string, Image> flags = new Dictionary<string, Image>();

    public Dictionary<GameObject, FlagData> selectedFlags = new Dictionary<GameObject, FlagData>();

    private void Start()
    {
        StartCoroutine(LoadPremadeImages());
    }

    private IEnumerator LoadPremadeImages()
    {
        var directory = new DirectoryInfo(Application.dataPath + "/Save Files/Saved Flags/");
        var flagsInSave = directory.GetFiles("*.png");

        for (int i = 0; i < flagsInSave.Length; i++)
        {
            var flag = AssetDatabase.LoadAssetAtPath<Sprite>(
                $"Assets/Save Files/Saved Flags/{flagsInSave[i].Name.Trim()}");
            CreateSavedPrefab(flagsInSave[i].Name, flag.texture, false);
            yield return new WaitForEndOfFrame();
        }
    }

    public void OpenSavePanel()
    {
        foreach (var pixel in gridManager.pixels.Values)
        {
            pixel.GetComponent<PixelManager>().canDraw = false;
        }
        
        savePanel.SetActive(true);
    }

    public void SaveFlag()
    {
        var saveName = saveNameText.text;

        if (saveName.ToCharArray().Length > 3)
            TakeAScreenShot(saveName);
    }

    public void CloseSavePanel()
    {
        saveNameText.text = String.Empty;
        savePanel.SetActive(false);
        foreach (var pixel in gridManager.pixels.Values)
        {
            pixel.GetComponent<PixelManager>().canDraw = true;
        }
    }

    public void TakeAScreenShot(string saveName)
    {
        saveName = ReplaceInvalidChars(saveName);

        var rt = screenShotCamera.targetTexture;
        RenderTexture.active = rt;

        // Encode texture
        var tex = EncodeTexture(rt);

        //Save in memory
        // string currentDateTime = DateTime.Now.ToString("MM/dd/yyyy");
        var directory = new DirectoryInfo(Application.dataPath + "/Save Files/Saved Flags/...");
        var path = Path.Combine(directory.Parent.FullName, $"{saveName}.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());

        saveNameText.text = String.Empty;

        //If image is in the list don't create new prefab
        if (flags != null || flags.Count > 0)
        {
            foreach (var flag in flags)
            {
                if (flag.Key != saveName) continue;
            
                RenderTexture.active = rt;
                flag.Value.sprite = GetSpriteFromTexture(tex);
                savePanel.SetActive(false);
                
                return;
            }
        }

        CreateSavedPrefab(saveName, tex, true);
    }

    private Texture2D EncodeTexture(RenderTexture rt)
    {
        var tex = new Texture2D(rt.width, rt.height);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private void CreateSavedPrefab(string saveName, Texture2D tex, bool createScriptableObject)
    {
        var flag = Instantiate(flagPrefab, savedFlagsGroup);
        var img = flag.transform.GetChild(0).GetComponent<Image>();
        var flagManager = flag.GetComponent<FlagManager>();
        
        img.sprite = GetSpriteFromTexture(tex);

        savePanel.SetActive(false);
        flags.Add(saveName, img);
        
        if (createScriptableObject)
        {
            flagManager.flagData = (FlagData)AssetDatabase.LoadAssetAtPath($"Assets/Save Files/Saved Flags/{saveName}.png", typeof(FlagData));
            CreateScriptableData(flag, saveName, img.sprite.texture);
        }
        else
        {
            var nameWithoutExtention = saveName.Substring(0, saveName.IndexOf(".png"));
            flagManager.flagData = (FlagData)AssetDatabase.LoadAssetAtPath($"Assets/Save Files/Saved Flags/{nameWithoutExtention}.asset", typeof(FlagData));
        }
        
        TurnDrawingBackOn();
    }
    
    private void CreateScriptableData(GameObject flag, string saveName, Texture flagTexture)
    {
        var flagManager = flag.GetComponent<FlagManager>();

        FlagData flagData = ScriptableObject.CreateInstance<FlagData>();
        flagData.name = saveName;
        flagData.saveUrl = $"Assets/Save Files/Saved Flags/{saveName}.asset";
        flagData.flag = flagTexture;

        foreach (var pixel in gridManager.pixels)
        {
            flagData.Colors.Add(pixel.Value.GetComponent<SpriteRenderer>().color);
        }

        AssetDatabase.CreateAsset(flagData, $"Assets/Save Files/Saved Flags/{saveName}.asset");
        AssetDatabase.SaveAssets();

        flagManager.flagData = flagData;
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private Sprite GetSpriteFromTexture(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 64f);
    }

    private void TurnDrawingBackOn()
    {
        foreach (var pixel in gridManager.pixels.Values)
        {
            pixel.GetComponent<PixelManager>().canDraw = true;
        }
    }
    
    public string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));    
    }

    public void DeletedFlags()
    {
        foreach (var flag in selectedFlags)
        {
            var flagData = flag.Value;

            if (!File.Exists(flagData.saveUrl))
            {
                print($"No file found with {flagData.saveUrl} name..");
            }
            else
            {
                var imageUrl = flagData.saveUrl.Substring(0, flagData.saveUrl.IndexOf(".asset")) + ".png";
                if (File.Exists(imageUrl))
                {
                    File.Delete(imageUrl);
                }
                else
                {
                    print($"No file found with {imageUrl} name..");
                }
                
                File.Delete(flagData.saveUrl);
                Destroy(flag.Key);
                
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }
        
        selectedFlags.Clear();
    }

    public void SelectFlag()
    {
        //TODO: Alert player to only select 1 flag to equip
        foreach (var flag in selectedFlags)
        {
            GameStateGO.GameState.flag = flag.Value.flag;
            return;
        }
        
        GameStateGO.GameState.Saving();
    }
}
