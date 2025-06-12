using System;
using System.Collections.Generic;
using Dawnshard.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    [RequireComponent(typeof(Slider))]
    public class SliderCardsAnimation : MonoBehaviour, IPointerUpHandler
    {
        private Slider slider;
        [SerializeField] private int cardsToShow = 5;
        [SerializeField] private List<GameObject> cards = new List<GameObject>();

        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(ScrollToPlace);
            AddCardList(cards);
        }

        private void OnEnable()
        {
            slider.value = 0;
            if (cards == null || cards.Count == 0)
            {
                slider.gameObject.SetActive(false);
            }
        }

        public void ScrollToPlace(float t)
        {
            ShowCardWithAnimation(t);
        }

        public void AddCardList(List<GameObject> cards)
        {
            this.cards = cards;
            slider.gameObject.SetActive(true);
            cards.Reverse();
            foreach (GameObject card in cards)
            {
                card.GetComponent<Animator>().enabled = true;
                card.GetComponent<CardFlipper>().enabled = true;
                card.transform.localScale = 3 * Vector3.one;
            }
            if (cards.Count <= cardsToShow)
            {
                slider.gameObject.SetActive(false);
                ShowCardWithAnimation(0);
            }
            else
            {
                slider.gameObject.SetActive(true);
                slider.maxValue = cards.Count - cardsToShow;
                ShowCardWithAnimation(0);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SliderValueToClosestIntValue();
        }

        private void ShowCardWithAnimation(float i)
        {
            if (cards==null || cards.Count == 0)
                return;
            for (int j = 0; j < cards.Count; j++)
            {
                if(cards[j]==null)
                    continue;
                if (!cards[j].TryGetComponent<Animator>(out var animator))
                    continue;
                float normalizedTime = 1f - (float)(j - i + 1) / (cardsToShow + 1f);
                animator.Play("Scroll.CardScroll", 0, normalizedTime);
                animator.speed = 0;
            }
        }

        private void SliderValueToClosestIntValue()
        {
            slider.value = Mathf.RoundToInt(slider.value);
        }

        public void Clear()
        {
            cards.Clear();
        }
    }
}
