using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }

    [System.Serializable]
    public struct GameInfo
    {
        public int matchCount;
        public int moveCount;
        public float timeElapsed;
        public int difficultyLevel;
        public bool isGameOver;
    }

    private GameInfo _gameInfo;

    public event Action<int> OnMatchCountUpdated;
    public event Action<int> OnMoveCountUpdated;
    public event Action<GameInfo> OnGameStarted;
    public event Action<GameInfo> OnGameEnded;

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
        _gameInfo = new GameInfo
        {
            matchCount = 0,
            moveCount = 0,
            timeElapsed = 0f,
            difficultyLevel = Mathf.Clamp(difficultyLevel, 1, 5),
            isGameOver = false
        };
        
        OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
        OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
        OnGameStarted?.Invoke(_gameInfo);
    }

    private void Update()
    {
        if (!_gameInfo.isGameOver)
        {
            _gameInfo.timeElapsed += Time.deltaTime;
        }
    }

    public void EndGame()
    {
        _gameInfo.isGameOver = true;
        OnGameEnded?.Invoke(_gameInfo);
    }

    public void AddMatch()
    {
        if (!_gameInfo.isGameOver)
        {
            _gameInfo.matchCount++;
            OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
        }
    }

    public void AddMove()
    {
        if (!_gameInfo.isGameOver)
        {
            _gameInfo.moveCount++;
            OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
        }
    }

    public int GetMatchCount()
    {
        return _gameInfo.matchCount;
    }

    public int GetMoveCount()
    {
        return _gameInfo.moveCount;
    }

    public float GetTimeElapsed()
    {
        return _gameInfo.timeElapsed;
    }

    public int GetDifficultyLevel()
    {
        return _gameInfo.difficultyLevel;
    }

    public bool IsGameOver()
    {
        return _gameInfo.isGameOver;
    }
}