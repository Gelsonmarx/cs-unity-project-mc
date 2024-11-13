using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic
{
    public class GameManager : Singleton<GameManager>
    {
        public event Action<List<int>, int> OnBoardCreated;
        private int gridSize;
        private Card currentSelectedCard = null;
        private List<int> _cardPairs;
        private bool isSelecting = false;
        [SerializeField] private List<Sprite> cardSprites;

        [System.Serializable]
        public struct GameInfo
        {
            public int matchCount;
            public int moveCount;
            public int difficultyLevel;
            public List<int> cardPairs;
            public List<int> filledPairs;
        }

        private GameInfo _gameInfo;

        public event Action<int> OnMatchCountUpdated;
        public event Action OnMissMatch;
        public event Action OnFlipCard;
        public event Action OnMatch;
        public event Action<int> OnMoveCountUpdated;
        public event Action<GameInfo> OnGameEnded;
        public event Action<GameInfo> OnRestartGame;
        public event Action<GameInfo> OnContinueGame;


        public void StartGame(int difficultyLevel)
        {
            ResetGameInfo();
            _gameInfo = new GameInfo
            {
                matchCount = 0,
                moveCount = 0,
                difficultyLevel = Mathf.Clamp(difficultyLevel, 0, 5),
                cardPairs = InitializeBoard(difficultyLevel, cardSprites),
                filledPairs = new List<int>()
            };
            OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
            OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
        }

        public void ContinueGame(GameInfo gameInfo)
        {
            _gameInfo = new GameInfo
            {
                matchCount = gameInfo.matchCount,
                moveCount = gameInfo.moveCount,
                difficultyLevel = gameInfo.difficultyLevel,
                cardPairs = ContinueBoard(gameInfo),
                filledPairs = gameInfo.filledPairs
            };
            OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
            OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
            OnContinueGame?.Invoke(_gameInfo);
        }

        public Sprite GetSpriteWithIndex(int index) => cardSprites[index];

        public void RestartGame()
        {
            OnRestartGame?.Invoke(_gameInfo);
        }

        void EndGame()
        {
            OnGameEnded?.Invoke(_gameInfo);
            ResetGameInfo();
        }

        private void ResetGameInfo()
        {
            int currentDificuty = _gameInfo.difficultyLevel;
            _gameInfo
              = new  GameInfo
            {
                matchCount = 0,
                moveCount = 0,
                difficultyLevel = currentDificuty,
                cardPairs = new List<int>(),
                filledPairs = new List<int>()
            };
            SaveLoadSystem.Instance.SaveGameProgress(_gameInfo);
        }

        void AddMatch()
        {
            _gameInfo.matchCount++;
            OnMatchCountUpdated?.Invoke(_gameInfo.matchCount);
            int matchSize = gridSize * (gridSize + 1) / 2;
            if (_gameInfo.matchCount >= matchSize)
            {
                EndGame();
            }
        }

        void AddMove()
        {
            _gameInfo.moveCount++;
            OnMoveCountUpdated?.Invoke(_gameInfo.moveCount);
        }


        [System.Serializable]
        public struct CardInfo
        {
            public int id;
        }

        public List<int> InitializeBoard(int difficultyLevel, List<Sprite> _cardSprites)
        {
            gridSize = GetGridSizeByDifficulty(difficultyLevel);
            GenerateCardPairs(_cardSprites);
            OnBoardCreated?.Invoke(_cardPairs, gridSize);
            return _cardPairs;
        }

        private List<int> ContinueBoard(GameInfo gameInfo)
        {
            _cardPairs = gameInfo.cardPairs;
            gridSize = GetGridSizeByDifficulty(gameInfo.difficultyLevel);
            OnBoardCreated?.Invoke(_cardPairs, gridSize);
            return _cardPairs;
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
            _cardPairs = new List<int>();

            for (int i = 0; i < numPairs; i++)
            {
                Sprite sprite = cardSprites[i % cardSprites.Count];
                _cardPairs.Add(i);
                _cardPairs.Add(i);
            }

            ShuffleCards(_cardPairs);
        }

        private void ShuffleCards(List<int> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int rnd = Random.Range(0, cards.Count);
                (cards[i], cards[rnd]) = (cards[rnd], cards[i]);
            }
        }

        public IEnumerator CheckMatchLogic(Card cardComponent, Card cardComponent2)
        {
            AddMove();
            yield return new WaitForSeconds(1f);

            if (cardComponent2.GetId() != cardComponent.GetId())
            {
                cardComponent2.TurnCard();
                cardComponent.TurnCard();
                OnMissMatch?.Invoke();
            }
            else
            {
                OnMatch?.Invoke();
                cardComponent2.DisableCard();
                cardComponent.DisableCard();
                _gameInfo.filledPairs.Add(cardComponent.GetId());
                AddMatch();
            }

            SaveLoadSystem.Instance.SaveGameProgress(_gameInfo);
        }

        public void OnCardClicked(Card cardComponent)
        {
            cardComponent.TurnCard();
            OnFlipCard?.Invoke();
            if (currentSelectedCard == null)
            {
                currentSelectedCard = cardComponent;
                return;
            }

            StartCoroutine(CheckMatchLogic(cardComponent, currentSelectedCard));
            currentSelectedCard = null;
        }
    }
}