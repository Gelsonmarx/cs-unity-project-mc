using Logic;
using Sound;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] private Button[] dificultyButtons;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button closeSettingsButton;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle muteSoundToggle;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject dificultyPanel;

        // Start is called before the first frame update
        void Start()
        {
            playButton.onClick.AddListener(StartGame);
            closeSettingsButton.onClick.AddListener(CloseSettings);
            settingsButton.onClick.AddListener(OpenSettings);

            SetSelectedDificulty();
            GameManager.Instance.OnRestartGame += info => OnDificultyChange(info.difficultyLevel);
        }

        private void OpenSettings()
        {
            settingsPanel.SetActive(true);
            settingsButton.gameObject.SetActive(false);
            dificultyPanel.gameObject.SetActive(false);
            volumeSlider.value = SaveLoadSystem.Instance.GameSettings.Volume;
            muteSoundToggle.isOn  = SaveLoadSystem.Instance.GameSettings.Mute;
            volumeSlider.onValueChanged.AddListener(slider => SoundManager.Instance.SetEffectsVolume(slider));
            muteSoundToggle.onValueChanged.AddListener(toggle => SoundManager.Instance.ToggleEffectsMute(toggle));
        }

        private void CloseSettings()
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            muteSoundToggle.onValueChanged.RemoveAllListeners();
            settingsPanel.SetActive(false);
            settingsButton.gameObject.SetActive(true);
            dificultyPanel.gameObject.SetActive(true);
            SetSelectedDificulty();
        }

        private void StartGame()
        {
            gameScreen.SetActive(true);
            int actualDificulty = SaveLoadSystem.Instance.GameSettings.Dificulty;
            GameManager.Instance.StartGame(actualDificulty);
        }

        void SetSelectedDificulty()
        {
            int actualDificulty = SaveLoadSystem.Instance.GameSettings.Dificulty;

            for (int index = 0; index < dificultyButtons.Length; index++)
            {
                int capturedIndex = index;
                if (index == actualDificulty)
                    dificultyButtons[index].Select();
                dificultyButtons[index].onClick.AddListener(() => { OnDificultyChange(capturedIndex); });
            }
        }

        void OnDificultyChange(int index)
        {
            SaveLoadSystem.Instance.SaveDificulty(index);

            SetSelectedDificulty();
        }
    }
}