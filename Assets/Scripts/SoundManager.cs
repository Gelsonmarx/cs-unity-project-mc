using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource effectsSource;
    [Range(0f, 1f)]
    [SerializeField] private float effectsVolume = 0.5f;

    [SerializeField] private List<AudioClip> soundEffects;
    private const string SOUND_VOLUME_PAYLOAD = "SoundVolume";
    private const string SOUND_MUTE_PAYLOAD = "SoundMute";

    private void Start()
    {
        var volume = PlayerPrefs.GetFloat(SOUND_VOLUME_PAYLOAD, effectsVolume);
        var mute = PlayerPrefs.GetInt(SOUND_MUTE_PAYLOAD, 1) == 0;
        effectsSource.mute = mute;
        SetEffectsVolume(volume);
    }
    

    public void PlayEffect(int effectIndex)
    {
        if (effectIndex >= 0 && effectIndex < soundEffects.Count)
        {
            effectsSource.PlayOneShot(soundEffects[effectIndex], effectsVolume);
        }
    }

    public void SetEffectsVolume(float volume)
    {
        effectsVolume = Mathf.Clamp01(volume);
        effectsSource.volume = effectsVolume;
        PlayerPrefs.SetFloat(SOUND_VOLUME_PAYLOAD, effectsVolume);
    }
    

    public void ToggleEffectsMute()
    {
        effectsSource.mute = !effectsSource.mute;
        int value = effectsSource.mute ? 0 : 1;
        PlayerPrefs.SetInt(SOUND_MUTE_PAYLOAD, value);
    }
}
