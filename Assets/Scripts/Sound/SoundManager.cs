using System.Collections.Generic;
using Logic;
using UnityEngine;

namespace Sound
{
    public class SoundManager : Singleton<SoundManager>
    {

        [SerializeField] private AudioSource effectsSource;

        [SerializeField] private List<AudioClip> soundEffects;

        private void Start()
        {
            var volume = SaveLoadSystem.Instance.GameSettings.Volume;
            var mute = SaveLoadSystem.Instance.GameSettings.Mute;
            effectsSource.mute = mute;
            SetEffectsVolume(volume);

            GameManager.Instance.OnGameEnded += info => PlayEffect(3);
            GameManager.Instance.OnFlipCard += () => PlayEffect(0);
            GameManager.Instance.OnMissMatch += () => PlayEffect(2);
            GameManager.Instance.OnMatch += () => PlayEffect(1);
        
        }
    

        public void PlayEffect(int effectIndex)
        {
            if (effectIndex >= 0 && effectIndex < soundEffects.Count)
            {
                effectsSource.PlayOneShot(soundEffects[effectIndex], SaveLoadSystem.Instance.GameSettings.Volume);
            }
        }

        public void SetEffectsVolume(float volume)
        {
            effectsSource.volume = volume;
            SaveLoadSystem.Instance.SaveSoundVolume(volume);
        }
    

        public void ToggleEffectsMute(bool value)
        {
            effectsSource.mute = value;
            SaveLoadSystem.Instance.SaveMute(effectsSource.mute);
        }
    }
}
