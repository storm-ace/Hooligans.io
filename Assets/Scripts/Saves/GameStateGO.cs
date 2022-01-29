using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saving
{
    public class GameStateGO : MonoBehaviour
    {
        public static GameState GameState;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            
            GameState = AssetDatabase.LoadAssetAtPath<GameState>(
                $"Assets/Save Files/SaveData.asset");
        }
    }
}
