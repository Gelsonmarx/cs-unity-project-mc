using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadSystem : Singleton<SaveLoadSystem>
{
    private const string SAVE_DIFICULTY_PAYLOAD = "Dificulty";
    private const string SOUND_VOLUME_PAYLOAD = "SoundVolume";
    private const string SOUND_MUTE_PAYLOAD = "SoundMute";
    
    public GameSettings GameSettings { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        int dificulty =  PlayerPrefs.GetInt(SAVE_DIFICULTY_PAYLOAD, 0);
        var volume = PlayerPrefs.GetFloat(SOUND_VOLUME_PAYLOAD, 1);
        var mute = PlayerPrefs.GetInt(SOUND_MUTE_PAYLOAD, 1) == 0;

        GameSettings = new GameSettings(dificulty,volume, mute);
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