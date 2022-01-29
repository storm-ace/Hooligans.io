using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Drawing
{
    public class GridManager : MonoBehaviour
    {
        public GameObject pixelObject;
        
        private int gridSize = 64;

        public Dictionary<Vector2, GameObject> pixels = new Dictionary<Vector2, GameObject>();

        [SerializeField] private static Color currentColor;

        public static Color CurrentColor => currentColor;

        private bool canLoadNewImage = true;

        private void Start()
        {
            ColorUtility.TryParseHtmlString("#0073FF", out Color outputColor);
            currentColor = outputColor;
            
            StartCoroutine(SpawnGrid());
        }

        private IEnumerator SpawnGrid()
        {
            int pixelPerDelay = 0;
            
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var obj = Instantiate(pixelObject, new Vector3(x, y, 0), quaternion.identity, transform);
                    pixels.Add(new Vector2(x, y), obj);
                    pixelPerDelay--;

                    if (pixelPerDelay >= 10)
                    {
                        pixelPerDelay = 0;
                        yield return new WaitForEndOfFrame();   
                    }
                }
            }
        }

        public void DeletedSavedFlags()
        {
            foreach (var flag in pixels.Values)
            {
                Destroy(flag);
            }
        }

        public IEnumerator LoadFlag(FlagData flagData)
        {
            if (!canLoadNewImage)
            {
                print("Image is still loading!");
                yield break;
            }
        
            canLoadNewImage = false;
            int jobsPerDelay = 100;
            int index = 0;

            foreach (var pixel in pixels)
            {
                pixel.Value.GetComponent<SpriteRenderer>().color = flagData.Colors[index];
                index++;
                jobsPerDelay--;
                
                if (jobsPerDelay <= 0)
                {
                    jobsPerDelay = 100;
                    yield return new WaitForEndOfFrame();   
                }
            }
        
            canLoadNewImage = true;
        }

        public void SetPencilColor(string colorHex)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color outputColor);
            currentColor = outputColor;
        }

        public void ClearField()
        {
            int pixelPerDelay = 0;

            print("Clearing...");
            
            foreach (var pixel in pixels.Values)
            {
                pixel.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
