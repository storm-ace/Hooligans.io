using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flag
{
    public class FollowMouse : MonoBehaviour
    {
        public Camera cameraDrawings;

        private float _currentBrushSize = 1;
        [SerializeField] private float maxBrushSize = 10;
        [SerializeField] private float minBrushSize = 1;

        private void Update()
        {
            var vec = GetMouseWorldPositionWithZ(Input.mousePosition, cameraDrawings);
            vec.z = 0f;

            transform.position = vec;

            ChangebrushSize();
        }
        
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            var worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        private void ChangebrushSize()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                _currentBrushSize += 1f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                _currentBrushSize -= 1f;
            }
            
            _currentBrushSize = Mathf.Clamp(_currentBrushSize, minBrushSize, maxBrushSize);

            transform.localScale = new Vector3(_currentBrushSize * 3, _currentBrushSize * 3, 1);
        }
    }

}