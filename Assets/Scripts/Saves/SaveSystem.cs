using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Flag;
using UnityEngine;

namespace Saves
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem instance;
        
        public GameData gameData;

        public Sprite currentFlag;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);

            instance = this;
        }

        private void Start()
        {
            gameData = LoadData();
        }

        public void SaveData(bool wipeSave = false)
        {
            UpdateLandData();

            print("Saving data...");

            var wipedData = new GameData()
            {
                continueGame = false,
                coins = 100
            };
            
            var jsonData = JsonUtility.ToJson(wipeSave ? wipedData : gameData);
            PlayerPrefs.SetString("SaveData", jsonData);
            PlayerPrefs.Save();

            if (wipeSave) gameData = wipedData;

            // var formatter = new BinaryFormatter();
            // var path = Application.persistentDataPath + "/SaveData.io";
            // var stream = new FileStream(path, FileMode.Create);
            //
            // formatter.Serialize(stream, wipeSave ? new GameData() : gameData);
            // stream.Close();
        }

        private GameData LoadData()
        {
            // var path = Application.persistentDataPath + "/SaveData.io";
            //
            // if (File.Exists(path))
            // {
            //     using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //     var formatter = new BinaryFormatter();
            //     return formatter.Deserialize(stream) as GameData;
            // }
            //
            // //If no data was found we create a new one
            // print("No file found creating one....");

            var jsonData = PlayerPrefs.GetString("SaveData");
            return JsonUtility.FromJson<GameData>(jsonData);

            ResetSave();
            return null;
        }

        [Description("Resets the data with a empty GameData")]
        public void ResetSave()
        {
            print("Resetting game data");
            SaveData(true);
        }

        public void UpdateLandData()
        {
            gameData.worldData.Clear();

            foreach (var landData in TerrainMaster.Instance.landTiles)
            {
                var land = new GameData.MapData()
                {
                    unitClaimed = landData.unitClaimed,
                    unitPower = landData.unitPower,
                    inCombat = landData.inCombat,
                    landID = landData.gameObject.name,
                    unitAvailable = landData.unitAvailable
                };

                gameData.worldData.Add(land);
            }
        }

        public void UpdateCombatData(string landUnderAttack)
        {
            gameData.attackingLand = landUnderAttack;
        }

        public List<GameData.FlagData> LoadImages()
        {
            return gameData.flagData.ToList();
        }

        public void SaveFlagData(GameData.FlagData newFlag)
        {
            gameData.flagData.Add(newFlag);
            SaveData();
        }
    }
}