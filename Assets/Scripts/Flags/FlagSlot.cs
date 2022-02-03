using Flags;
using Saves;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Flag
{
    public class FlagSlot : MonoBehaviour, IPointerDownHandler
    {
        public Toggle toggle;
        public InputField flagInputField;
        
        private GridManager gridManager;

        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            flagInputField.text = SaveSystem.instance.gameData.name;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                StartCoroutine(gridManager.LoadFlag(SaveSystem.instance.gameData));
                eventData.clickCount = 0;
            }
        }

        public void ClickToggle()
        {
            
        }

        public void ChangeFlagName()
        {
            if (flagInputField.text.Length < 3 || flagInputField.text == SaveSystem.instance.gameData.name) return;

            // if (saveSucces) flagData = AssetDatabase.LoadAssetAtPath<FlagData>(
            //     $"Assets/Drawing/Flags/Saved/{flagInputField.text}.asset");
        }
    }

}