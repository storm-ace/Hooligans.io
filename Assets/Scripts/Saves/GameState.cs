using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saving
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "Hooligans.io/Saving")]
    public class GameState : ScriptableObject
    {
        public List<MapData> worldData = new List<MapData>();
        public string attackingLand;
        public string defendingLand;
        public bool newGame;
        public int coins;
        public int level;

        public bool fightWon = false;
        public Texture flag;

        public void SaveWorldData(List<LandChunk> worldData, string attackingLand = null, string defendingLand = null)
        {
            this.worldData.Clear();

            this.attackingLand = attackingLand;
            this.defendingLand = defendingLand;

            for (int i = 0; i < worldData.Count; i++)
            {
                var data = new MapData()
                {
                    unitClaimed = worldData[i].unitClaimed,
                    unitPower = worldData[i].unitPower,
                    inCombat = worldData[i].inCombat,
                    landID = worldData[i].gameObject.name
                };
                
                this.worldData.Add(data);
            }
        }

        public void Saving()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
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
    }
}