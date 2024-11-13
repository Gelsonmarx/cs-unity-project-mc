using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameBoardUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform boardParent;
        [SerializeField] private TextMeshProUGUI matchCountText;
        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private TextMeshProUGUI endGameMoveCount;
        [SerializeField] private GameObject endGameScreen;
        [SerializeField] private Button endGameButton;

        private GameManager gameManager;
        private int gridSize;

        private void OnEnable()
        {
            gameManager = GameManager.Instance;
            gameManager.OnBoardCreated += HandleBoardCreated;
            gameManager.OnMatchCountUpdated += HandleMatchCount;
            gameManager.OnMoveCountUpdated += HandleMoveCount;
            gameManager.OnGameEnded += HandleEndGame;
            gameManager.OnContinueGame += HandleContinueGame;
            endGameButton.onClick.AddListener(() =>
            {
                endGameScreen.SetActive(false);
                this.gameObject.SetActive(false);
                endGameButton.onClick.RemoveAllListeners();
                gameManager.RestartGame();
            });
        }

        private void HandleContinueGame(GameManager.GameInfo gameInfo)
        {
            foreach (Transform child in boardParent)
            {
                var card = child.gameObject.GetComponent<Card>();
                if (gameInfo.filledPairs.Contains(card.GetId()))
                {
                    card.DisableCard();
                }
            }
        }

        private void HandleEndGame(GameManager.GameInfo info)
        {
            endGameScreen.SetActive(true);
            endGameMoveCount.text =  $"You finished the game with { info.moveCount.ToString()} moves!";
        }

        private void HandleMatchCount(int count)
        {
            matchCountText.text = count.ToString();
        }

        private void HandleMoveCount(int count)
        {
            moveCountText.text = count.ToString();
        }

        private void HandleBoardCreated(List<int> cardPairs, int gridSize)
        {
            this.gridSize = gridSize;
            CreateBoard(cardPairs);
        }

        private void CreateBoard(List<int> cardPairs)
        {
            foreach (Transform child in boardParent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < cardPairs.Count; i++)
            {
                GameObject card = Instantiate(cardPrefab, boardParent);
                card.transform.localScale = Vector3.one;

                int cardID = cardPairs[i];
                Card cardComponent = card.GetComponent<Card>();
                var sprite = gameManager.GetSpriteWithIndex(cardID);
                cardComponent.Init(cardID, sprite);

                Button cardButton = card.GetComponent<Button>();
                if (cardButton != null)
                {
                    cardButton.onClick.AddListener(() => gameManager.OnCardClicked(cardComponent));
                }
            }

            AdjustBoardLayout();
        }

        private void AdjustBoardLayout()
        {
            var gridLayout = boardParent.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                float boardWidth = boardParent.GetComponent<RectTransform>().rect.width;
                float boardHeight = boardParent.GetComponent<RectTransform>().rect.height;

                bool isLandscape = Screen.width > Screen.height;

                float horizontalSpacing = gridLayout.spacing.x * (gridSize);
                float verticalSpacing = gridLayout.spacing.y * (gridSize - 1);

                float cellWidth, cellHeight;

                if (isLandscape)
                {
                    cellWidth = (boardWidth - horizontalSpacing) / (gridSize + 1);
                    cellHeight = (boardHeight - verticalSpacing) / gridSize;
                }
                else
                {
                    cellWidth = (boardWidth - horizontalSpacing) / (gridSize + 1);
                    cellHeight = (boardHeight - verticalSpacing) / gridSize;
                }

                float cellSize = Mathf.Min(cellWidth, cellHeight);

                gridLayout.cellSize = new Vector2(cellSize, cellSize);
                gridLayout.constraintCount = gridSize;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.OnBoardCreated -= HandleBoardCreated;
                gameManager.OnMatchCountUpdated -= HandleMatchCount;
                gameManager.OnMoveCountUpdated -= HandleMoveCount;
                gameManager.OnGameEnded -= HandleEndGame;
                gameManager.OnContinueGame -= HandleContinueGame;
            }
        }
    }
}