using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameUIHandler : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform boardParent;
    public List<Sprite> cardSprites;

    private int gridSize;

    private Card currentSelectedCard = null;

    [System.Serializable]
    public struct CardInfo
    {
        public int id;
        public Sprite sprite;
    }

    private List<CardInfo> _cardPairs;
    private bool isSelecting = false;

    private void Start()
    {
        InitializeBoard(GameManager.Instance.GetDifficultyLevel());
    }

    public void InitializeBoard(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1:
                gridSize = 2;
                break;
            case 2:
                gridSize = 3;
                break;
            case 3:
                gridSize = 4;
                break;
            case 4:
                gridSize = 5;
                break;
            case 5:
                gridSize = 6;
                break;
            default:
                gridSize = 2;
                break;
        }

        GenerateCardPairs();
        CreateBoard(gridSize);
    }

    private void GenerateCardPairs()
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

    private void CreateBoard(int gridSize)
    {
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _cardPairs.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, boardParent);
            card.transform.localScale = Vector3.one;

            CardInfo cardInfo = _cardPairs[i];
            Card cardComponent = card.GetComponent<Card>();

            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClickedHandler(cardComponent));
            }
        }

        AdjustBoardLayout(gridSize);
    }

    private void OnCardClickedHandler(Card cardComponent)
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

    private IEnumerator CheckMatchLogic(Card cardComponent)
    {
        isSelecting = true;
        GameManager.Instance.AddMove();
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
            GameManager.Instance.AddMatch();
            currentSelectedCard = null;
        }

        isSelecting = false;
    }

    private void AdjustBoardLayout(int gridSize)
    {
        var gridLayout = boardParent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null) return;

        float boardWidth = boardParent.GetComponent<RectTransform>().rect.width;
        float boardHeight = boardParent.GetComponent<RectTransform>().rect.height;

        bool isLandscape = Screen.width > Screen.height;

        float horizontalSpacing = gridLayout.spacing.x * (gridSize);
        float verticalSpacing = gridLayout.spacing.y * (gridSize - 1);

        float cellWidth, cellHeight;

        if (isLandscape)
        {
            cellWidth = (boardWidth - horizontalSpacing) / (gridSize +1);
            cellHeight = (boardHeight - verticalSpacing) / gridSize;
        }
        else
        {
            cellWidth = (boardWidth - horizontalSpacing) / (gridSize +1);
            cellHeight = (boardHeight - verticalSpacing) / gridSize;
        }

        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        gridLayout.constraintCount = gridSize;
    }
}