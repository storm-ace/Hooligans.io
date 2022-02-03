using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flags;
using Saves;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Flag
{
    public class FlagManager : MonoBehaviour
    {
        public Camera screenShotCamera;

        public GameObject savePanel;

        public Transform savedFlagsGroup;

        public GameObject flagPrefab;

        public InputField saveNameText;

        [SerializeField] private GridManager gridManager;

        private Dictionary<string, Image> flags = new Dictionary<string, Image>();

        public Dictionary<GameObject, GameData> selectedFlags = new Dictionary<GameObject, GameData>();

        private void Start()
        {
            StartCoroutine(LoadPremadeImages());
        }

        private IEnumerator LoadPremadeImages()
        {
            var flagsInSave = SaveSystem.instance.LoadImages();

            for (int i = 0; i < flagsInSave.Count; i++)
            {
                var flag = flagsInSave[i];
                CreateSavedPrefab(flag.name, flag.flagTexture, false);
                yield return new WaitForEndOfFrame();
            }
        }

        public void OpenSavePanel()
        {
            foreach (var pixel in gridManager.Pixels.Values)
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
            foreach (var pixel in gridManager.Pixels.Values)
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

            //Save in file
            var colors = gridManager.Pixels.Select(t => t.Value.GetComponent<SpriteRenderer>().color).ToList();
            var colorsToFlagColors = new List<GameData.Color>();
            for (int i = 0; i < colors.Count; i++)
            {
                var color = new GameData.Color()
                {
                    r = colors[i].r,
                    b = colors[i].b,
                    g = colors[i].g,
                    a = colors[i].a
                };
                
                colorsToFlagColors.Add(color);
            }
            
            var newFlag = new GameData.FlagData()
            {
                name = saveName,
                flagTexture = tex,
                pixelColors = colorsToFlagColors
            };
            
            SaveSystem.instance.SaveFlagData(newFlag);
            
            //Clears the savewindow inputfield
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

            img.sprite = GetSpriteFromTexture(tex);

            savePanel.SetActive(false);
            flags.Add(saveName, img);

            TurnDrawingBackOn();
        }

        public static Sprite GetSpriteFromTexture(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), 64f);
        }

        private void TurnDrawingBackOn()
        {
            foreach (var pixel in gridManager.Pixels.Values)
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
            
        }

        public void SelectFlag()
        {
            //TODO: Alert player to only select 1 flag to equip
        }
    }
}