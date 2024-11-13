using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private Button[] dificultyButtons;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject gameScreen;

    private const string SAVE_PAYLOAD = "Dificulty";

    // Start is called before the first frame update
    void Start()
    {
        
        playButton.onClick.AddListener(StartGame);
        
        int actualDificulty = PlayerPrefs.GetInt(SAVE_PAYLOAD, 0);

        for (int index = 0; index < dificultyButtons.Length; index++)
        {
            int capturedIndex = index;
            if (index == actualDificulty)
                dificultyButtons[index].Select();
            dificultyButtons[index].onClick.AddListener(() => { OnDificultyChange(capturedIndex); });
        }
    }

    private void StartGame()
    {
        gameScreen.SetActive(true);
        int actualDificulty = PlayerPrefs.GetInt(SAVE_PAYLOAD, 0);
        GameManager.Instance.StartGame(actualDificulty);
    }

    void OnDificultyChange(int index)
    {
        PlayerPrefs.SetInt(SAVE_PAYLOAD, index);

        for (int i = 0; i < dificultyButtons.Length; i++)
        {
            if (index == i)
                dificultyButtons[index].Select();
        }
    }
}