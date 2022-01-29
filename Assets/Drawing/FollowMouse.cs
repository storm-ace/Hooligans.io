using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drawing
{
    public class FollowMouse : MonoBehaviour
    {
        public Camera cameraDrawings;

        private float currentBrushSize = 1;
        private float maxBrushSize = 10;
        private float minBrushSize = 1;

        private void Update()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, cameraDrawings);
            vec.z = 0f;

            transform.position = vec;

            ChangebrushSize();
        }
        
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        private void ChangebrushSize()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                currentBrushSize += 1f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                currentBrushSize -= 1f;
            }
            
            currentBrushSize = Mathf.Clamp(currentBrushSize, 1f, 10f);

            transform.localScale = new Vector3(currentBrushSize * 3, currentBrushSize * 3, 1);
        }
    }

}