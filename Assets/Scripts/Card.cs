using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject cardback;
    [SerializeField] private Button cardButton;
    private int id = 0;

    public void Init(GameUIHandler.CardInfo cardInfo)
    {
        this.id = cardInfo.id;
        cardImage.sprite = cardInfo.sprite;
    }

    public int GetId() => id;
    
    public void TurnCard()
    {
        bool isBackActive = cardback.gameObject.activeSelf;
        cardButton.interactable = !isBackActive;
        cardback.gameObject.SetActive(!isBackActive);
        
    }

    public void DisableCard()
    {
        cardback.gameObject.SetActive(false);
        cardImage.gameObject.SetActive(false);
        this.GetComponent<Image>().enabled = false;
    }
}
