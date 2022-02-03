using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flag
{
    [Serializable]
    public class GameData
    {
        public string name = "Data";
        
        public List<Color> colorPerPixels = new List<Color>();
        public List<MapData> worldData = new List<MapData>();
        public List<FlagData> flagData = new List<FlagData>();

        public string attackingLand;
        public string defendingLand;
        public int coins = 0;
        public int level = 1;
        public int enemyUnitStrenght;

        public bool fightWon = false;

        public string saveUrl = "";
        public bool continueGame = false, returnToMenu = false;
        
           
        [Serializable]
        public class Color
        {
            //Color RGBA
            public float r, g, b, a;
        
            public Color GetColorData()
            {
                return new Color()
                {
                    r = this.r,
                    g = this.g,
                    b = this.b,
                    a = this.a
                };
            }
        }
        
        [Serializable]
        public class MapData
        {
            public enum UnitClaimed {Unclaimed, Player, Hostile}
        
            public UnitClaimed unitClaimed;
            public int unitPower;
            public bool inCombat;
            public string landID;
            public bool unitAvailable;
        }
        
        [Serializable]
        public class FlagData
        {
            public string name;
            public Texture2D flagTexture;
            public List<Color> pixelColors;
        }
    }
}