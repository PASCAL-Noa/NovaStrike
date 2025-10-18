using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] public bool _mSoundBool;
    [SerializeField] public int _mSelectedBackGround;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Toggle Toggle;
    [SerializeField] List<Color> backgroundColors;
    [SerializeField] AudioManager audioSource;


    private string _mSavePath;
    public string SavePath { get => _mSavePath; set => _mSavePath = value; }

    private void Awake()
    {
        SavePath = Application.persistentDataPath + "/SettingsData.json";
    }

    void Start()
    {
        LoadSettings();
        dropdown.value = _mSelectedBackGround;
        Toggle.isOn = _mSoundBool;
        SetBackground();
        SetSounds();
        dropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });
    }

    void Update()
    {
        _mSelectedBackGround = dropdown.value;
    }

    void SetBackground()
    {
        if (backgroundColors != null && _mSelectedBackGround >= 0 && _mSelectedBackGround < backgroundColors.Count)
        {
            Camera.main.backgroundColor = backgroundColors[_mSelectedBackGround];
        }
    }

    void OnDropdownChanged()
    {
        _mSelectedBackGround = dropdown.value;
        SetBackground();
        SaveSettings();
    }

    public void SaveSettings()
    {
        SettingsData data = new SettingsData()
        {
            SoundsBool = _mSoundBool,
            SelectedBackGround = _mSelectedBackGround,
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
 
        if (GameManager.instance != null && GameManager.instance._mPlayer.ID != null)
            GameManager.instance._mPlayer.SavePlayer(DataSync.instance);
    }

    public void LoadSettings()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);
            _mSoundBool = data.SoundsBool;
            _mSelectedBackGround = data.SelectedBackGround;
            print("Loaded");
        }
        else
        {
            _mSoundBool = true;
            _mSelectedBackGround = 0;
            print("New");
        }
    }

    public void SwitchMsound()
    {
        _mSoundBool = !_mSoundBool;
        SetSounds();
    }
    public void SetSounds()
    {
        audioSource.SwitchIsNotMute(_mSoundBool);
        SaveSettings();
    }
}

[Serializable]
public class SettingsData
{
    public bool SoundsBool;
    public int SelectedBackGround;
}