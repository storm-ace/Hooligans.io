using System.Collections;
using System.Collections.Generic;
using Flag;
using Unity.Mathematics;
using UnityEngine;

namespace Flags
{
    public class GridManager : MonoBehaviour
    {
        public GameObject pixelObject;

        private const int GridSize = 64;

        public Dictionary<Vector2, GameObject> Pixels = new Dictionary<Vector2, GameObject>();

        private static Color _currentColor;

        public static Color CurrentColor => _currentColor;

        private bool _canLoadNewImage = true;

        private void Start()
        {
            ColorUtility.TryParseHtmlString("#0073FF", out Color outputColor);
            _currentColor = outputColor;
            
            StartCoroutine(SpawnGrid());
        }

        private IEnumerator SpawnGrid()
        {
            int pixelPerDelay = 0;
            
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    var obj = Instantiate(pixelObject, new Vector3(x, y, 0), quaternion.identity, transform);
                    Pixels.Add(new Vector2(x, y), obj);
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
            foreach (var flag in Pixels.Values)
            {
                Destroy(flag);
            }
        }

        public IEnumerator LoadFlag(GameData gameData)
        {
            if (!_canLoadNewImage)
            {
                print("Image is still loading!");
                yield break;
            }
        
            _canLoadNewImage = false;
            int jobsPerDelay = 100;
            int index = 0;

            foreach (var pixel in Pixels)
            {
                var color = new Color(gameData.colorPerPixels[index].r, gameData.colorPerPixels[index].b,
                    gameData.colorPerPixels[index].g, gameData.colorPerPixels[index].a);
                pixel.Value.GetComponent<SpriteRenderer>().color = color;
                index++;
                jobsPerDelay--;
                
                if (jobsPerDelay <= 0)
                {
                    jobsPerDelay = 100;
                    yield return new WaitForEndOfFrame();   
                }
            }
        
            _canLoadNewImage = true;
        }

        public void SetPencilColor(string colorHex)
        {
            ColorUtility.TryParseHtmlString(colorHex, out Color outputColor);
            _currentColor = outputColor;
        }

        public void ClearField()
        {
            print("Clearing...");
            
            foreach (var pixel in Pixels.Values)
            {
                pixel.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
