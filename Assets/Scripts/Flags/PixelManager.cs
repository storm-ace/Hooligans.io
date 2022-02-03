using System;
using System.Collections;
using System.Collections.Generic;
using Flags;
using UnityEngine;

namespace Flag
{
    public class PixelManager : MonoBehaviour
    {
        private SpriteRenderer sRend;

        public bool canDraw = true;

        private void Start()
        {
            sRend = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Brush") && canDraw)
            {
                if (Input.GetMouseButton(0))
                {
                    sRend.color = GridManager.CurrentColor;
                }

                if (Input.GetMouseButton(1))
                {
                    sRend.color = Color.white;
                }
            }
        }
    }
}