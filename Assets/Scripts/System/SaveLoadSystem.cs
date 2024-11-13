using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;

public class SaveLoadSystem : Singleton<SaveLoadSystem>
{
    private const string SAVE_DIFICULTY_PAYLOAD = "Dificulty";
    private const string SOUND_VOLUME_PAYLOAD = "SoundVolume";
    private const string SOUND_MUTE_PAYLOAD = "SoundMute";
    private const string GAME_PROGRESS_PAYLOAD = "GameProgress";
    

    public GameSettings GameSettings { get; private set; }
    public GameManager.GameInfo GameProgress;
    protected override void Awake()
    {
        base.Awake();
        int dificulty =  PlayerPrefs.GetInt(SAVE_DIFICULTY_PAYLOAD, 0);
        var volume = PlayerPrefs.GetFloat(SOUND_VOLUME_PAYLOAD, 1);
        var mute = PlayerPrefs.GetInt(SOUND_MUTE_PAYLOAD, 1) == 0;
        var progress = PlayerPrefs.GetString(GAME_PROGRESS_PAYLOAD, null);

        GameSettings = new GameSettings(dificulty,volume, mute);
        
        string progressJson = PlayerPrefs.GetString(GAME_PROGRESS_PAYLOAD, null);
        if (!string.IsNullOrEmpty(progressJson))
        {
            GameProgress = JsonUtility.FromJson<GameManager.GameInfo>(progressJson);
        }
        else
        {
            GameProgress = new GameManager.GameInfo();
        }
    }

    public void SaveGameProgress(GameManager.GameInfo gameInfo)
    {
        GameProgress = gameInfo;
        string progressJson = JsonUtility.ToJson(gameInfo);
        PlayerPrefs.SetString(GAME_PROGRESS_PAYLOAD, progressJson);
    }

    public void SaveDificulty(int newValue)
    {
        GameSettings = new GameSettings(newValue, GameSettings.Volume, GameSettings.Mute);
        PlayerPrefs.SetInt(SAVE_DIFICULTY_PAYLOAD, newValue);
    }
    
    public void SaveSoundVolume(float newValue)
    {
        GameSettings = new GameSettings(GameSettings.Dificulty, newValue, GameSettings.Mute);
        PlayerPrefs.SetFloat(SOUND_VOLUME_PAYLOAD, newValue);
    }
    
    public void SaveMute(bool newValue)
    {
        var muteInt = newValue ? 0 : 1;
        GameSettings = new GameSettings(GameSettings.Dificulty, GameSettings.Volume, newValue);
        PlayerPrefs.SetInt(SOUND_MUTE_PAYLOAD, muteInt);
    }
}

public struct GameSettings
{
    public int Dificulty;
    public float Volume;
    public bool Mute;

    public GameSettings(int dificulty, float volume, bool mute)
    {
        this.Dificulty = dificulty;
        this.Volume = volume;
        this.Mute = mute;
    }
}