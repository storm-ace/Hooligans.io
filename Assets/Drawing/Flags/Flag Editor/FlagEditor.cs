using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlagEditor : MonoBehaviour
{
    public Image rend;
    Texture2D texture;
    Color[] Data;
    public int Width;
    public int Height;

    void Start()
    {
        Width = 128;
        Height = 128;
        Data = texture.GetPixels();

        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    private void Update()
    {
        DebugPoint();
    }

    void DebugPoint()
    {
        if (Input.GetMouseButton(0))
        {

            // Get Mouse position - convert to world position
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPos = new Vector2(screenPos.x, screenPos.y);

            // Check if we clicked on our object
            RaycastHit2D[] ray = Physics2D.RaycastAll(screenPos, Vector2.zero, 0.01f);
            for (int i = 0; i < ray.Length; i++)
            {
                // You will want to tag the image you want to lookup
                if (ray[i].collider.tag == "FlagView")
                {
                    // Set click position to the gameobject area
                    screenPos -= ray[i].collider.gameObject.transform.position;
                    int x = (int)(screenPos.x * Width);
                    int y = (int)(screenPos.y * Height) + Height;

                    print(x);

                    // Get color data
                    if (x > 0 && x < Width && y > 0 && y < Height)
                    {
                        print(Data[y * Width + x]);
                        //Color = Data[y * Width + x];
                    }
                    break;
                }
            }
        }
    }
}
