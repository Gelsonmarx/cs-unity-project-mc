using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image cardImage;
        [SerializeField] private GameObject cardback;
        [SerializeField] private Button cardButton;
        private int id = 0;

        public void Init(int id, Sprite sprite)
        {
            this.id = id;
            cardImage.sprite = sprite;
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
}
