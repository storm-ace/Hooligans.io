using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drawing
{
    public class FlagManager : MonoBehaviour, IPointerDownHandler
    {
        public FlagData flagData;
        public Toggle toggle;
        public InputField flagInputField;
        
        private GridManager gridManager;
        private SaveAndStoreManager saveAndStore;

        private void Awake()
        {
            gridManager = FindObjectOfType<GridManager>();
            saveAndStore = FindObjectOfType<SaveAndStoreManager>();
        }

        private void Start()
        {
            flagInputField.text = flagData.name;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                StartCoroutine(gridManager.LoadFlag(flagData));
                eventData.clickCount = 0;
            }
        }

        public void ClickToggle()
        {
            if (toggle.isOn)
            {
                if (!saveAndStore.selectedFlags.ContainsValue(flagData)) saveAndStore.selectedFlags.Add(gameObject, flagData);
            } else if (saveAndStore.selectedFlags.ContainsValue(flagData)) saveAndStore.selectedFlags.Remove(gameObject);
        }

        public void ChangeFlagName()
        {
            if (flagInputField.text.Length < 3 || flagInputField.text == flagData.name) return;
            
            var saveSucces = flagData.SaveData(flagInputField.text);
            
            if (saveSucces) flagData = AssetDatabase.LoadAssetAtPath<FlagData>(
                $"Assets/Drawing/Flags/Saved/{flagInputField.text}.asset");
        }
    }

}