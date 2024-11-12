using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public struct GameInfo
    {
        public int score;
        public float timeElapsed;
        public int difficultyLevel;
        public bool isGameOver;
    }

    public GameInfo gameInfo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame(int difficultyLevel)
    {
        gameInfo = new GameInfo
        {
            score = 0,
            timeElapsed = 0f,
            difficultyLevel = Mathf.Clamp(difficultyLevel, 1, 5),
            isGameOver = false
        };
    }

    private void Update()
    {
        if (!gameInfo.isGameOver)
        {
            gameInfo.timeElapsed += Time.deltaTime;
        }
    }

    public void EndGame()
    {
        gameInfo.isGameOver = true;
    }

    public void AddScore(int points)
    {
        if (!gameInfo.isGameOver)
        {
            gameInfo.score += points;
        }
    }

    public int GetScore()
    {
        return gameInfo.score;
    }

    public float GetTimeElapsed()
    {
        return gameInfo.timeElapsed;
    }

    public int GetDifficultyLevel()
    {
        return gameInfo.difficultyLevel;
    }

    public bool IsGameOver()
    {
        return gameInfo.isGameOver;
    }
}