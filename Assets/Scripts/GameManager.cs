using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }
   
   public event Action<List<CardInfo>, int> OnBoardCreated;
   private int gridSize;
   private Card currentSelectedCard = null;
   private List<CardInfo> _cardPairs;
   private bool isSelecting = false;
   [SerializeField] private List<Sprite> cardSprites;

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
            difficultyLevel = Mathf.Clamp(difficultyLevel, 0, 5),
            isGameOver = false
        };
        InitializeBoard(difficultyLevel, cardSprites);
        OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
        OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
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
            int matchSize = gridSize * (gridSize + 1)/ 2;
            if (_gameInfo.matchCount >= matchSize)
            {
                EndGame();
            }
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
    

    [System.Serializable]
    public struct CardInfo
    {
        public int id;
        public Sprite sprite;
    }

    public void InitializeBoard(int difficultyLevel, List<Sprite> _cardSprites)
    {
        gridSize = GetGridSizeByDifficulty(difficultyLevel);
        GenerateCardPairs(_cardSprites);
        OnBoardCreated?.Invoke(_cardPairs, gridSize);  
    }

    private int GetGridSizeByDifficulty(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0: return 2;
            case 1: return 3;
            case 2: return 4;
            case 3: return 5;
            case 4: return 6;
            default: return 2;
        }
    }

    private void GenerateCardPairs(List<Sprite> cardSprites)
    {
        int numCards = gridSize * (gridSize + 1);
        int numPairs = numCards / 2;
        _cardPairs = new List<CardInfo>();

        for (int i = 0; i < numPairs; i++)
        {
            Sprite sprite = cardSprites[i % cardSprites.Count];
            CardInfo cardInfo = new CardInfo { id = i, sprite = sprite };
            _cardPairs.Add(cardInfo);
            _cardPairs.Add(cardInfo);
        }

        ShuffleCards(_cardPairs);
    }

    private void ShuffleCards(List<CardInfo> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rnd = Random.Range(0, cards.Count);
            (cards[i], cards[rnd]) = (cards[rnd], cards[i]);
        }
    }

    public IEnumerator CheckMatchLogic(Card cardComponent)
    {
        isSelecting = true;
        AddMove();
        yield return new WaitForSeconds(1f);

        if (currentSelectedCard.GetId() != cardComponent.GetId())
        {
            currentSelectedCard.TurnCard();
            cardComponent.TurnCard();
            currentSelectedCard = null;
        }
        else
        {
            currentSelectedCard.DisableCard();
            cardComponent.DisableCard();
            AddMatch();
            currentSelectedCard = null;
        }

        isSelecting = false;
    }

    public void OnCardClicked(Card cardComponent)
    {
        if (isSelecting) return;
        cardComponent.TurnCard();

        if (currentSelectedCard == null)
        {
            currentSelectedCard = cardComponent;
            return;
        }

        StartCoroutine(CheckMatchLogic(cardComponent));
    }
}