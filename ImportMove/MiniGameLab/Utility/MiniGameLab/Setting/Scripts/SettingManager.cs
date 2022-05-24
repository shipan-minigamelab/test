using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;

namespace CommonStuff
{
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance;
        private const string settingDataKey = "setting_data";
        
        public SettingData settingData;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //Load Data 
            // settingData = ;

            SettingMenu.Instance.OnItemButtonClicked += OnItemButtonClicked;
            SettingMenu.Instance.OnItemValueChange += OnItemValueChange;
            SettingMenu.Instance.OnMenuShowEvent += OnMenuShowEvent;

            LoadData();
        }

        private void OnMenuShowEvent(bool isShow)
        {
            if (isShow)
            {
                LoadData();
                SettingMenu.Instance.Show();
            }
            else
            {
                SettingMenu.Instance.Hide();
                SaveData();
            }
        }

        private void OnItemValueChange(SettingItemType itemType, float value)
        {
            UpdateValue(itemType, value);
        }

        private void OnItemButtonClicked(SettingItemType itemType)
        {
            switch (itemType)
            {
                case SettingItemType.HAPTIC:
                    UpdateValue(SettingItemType.HAPTIC, settingData.HapticValue ? 0 : 1);
                    break;

                case SettingItemType.MUSIC:
                    UpdateValue(SettingItemType.MUSIC, settingData.MusicValue > 0.1f ? 0 : 1);
                    break;

                case SettingItemType.SFX:
                    UpdateValue(SettingItemType.SFX, settingData.SfxValue > 0.1f ? 0 : 1);
                    break;
            }
        }


        public void UpdateValue(SettingItemType itemType, float value = 1)
        {
            switch (itemType)
            {
                case SettingItemType.HAPTIC:
                    if (value >= 0)
                        settingData.HapticValue = value > 0 ? true : false;
                    SettingMenu.Instance.SetItemValue(SettingItemType.HAPTIC, settingData.HapticValue ? 1 : 0);
                    VibrationManager.instance.ToggleVibration(settingData.HapticValue);

                    break;

                case SettingItemType.MUSIC:
                    if (value >= 0)
                        settingData.MusicValue = value;
                    SettingMenu.Instance.SetItemValue(SettingItemType.MUSIC, settingData.MusicValue);
                    AudioManager.Instance.SetVolume(AudioGroup.MUSIC, settingData.MusicValue);

                    break;

                case SettingItemType.SFX:
                    if (value >= 0)
                        settingData.SfxValue = value;
                    SettingMenu.Instance.SetItemValue(SettingItemType.SFX, settingData.SfxValue);
                    AudioManager.Instance.SetVolume(AudioGroup.SFX, settingData.SfxValue);

                    break;
            }
        }


        private void  LoadData()
        {
            if (SaveGame.Exists(settingDataKey))
            {
                settingData = SaveGame.Load<SettingData>(settingDataKey);
            }

            UpdateValue(SettingItemType.HAPTIC,1);
            UpdateValue(SettingItemType.SFX,1);
            UpdateValue(SettingItemType.MUSIC,1);
        }
        
        private void   SaveData()
        {
            SaveGame.Save(settingDataKey, settingData);
        }
        
    }

    [System.Serializable]
    public struct SettingData
    {
        public float MusicValue;
        public float SfxValue;
        public bool HapticValue;
    }
}