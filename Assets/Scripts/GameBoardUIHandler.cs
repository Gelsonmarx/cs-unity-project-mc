using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        endGameButton.onClick.AddListener(() =>
        {
            endGameScreen.SetActive(false);
            this.gameObject.SetActive(false);
            endGameButton.onClick.RemoveAllListeners();
        });
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

    private void HandleBoardCreated(List<GameManager.CardInfo> cardPairs, int gridSize)
    {
        this.gridSize = gridSize;
        CreateBoard(cardPairs);
    }

    private void CreateBoard(List<GameManager.CardInfo> cardPairs)
    {
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < cardPairs.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, boardParent);
            card.transform.localScale = Vector3.one;

            GameManager.CardInfo cardInfo = cardPairs[i];
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.Init(cardInfo);

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
        }
    }
}