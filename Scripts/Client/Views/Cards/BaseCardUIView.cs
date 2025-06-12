using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Presenters;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class BaseCardUIView : MonoBehaviour, ICardView, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// The card view shows the data of a card, exposing easy to access methods to modify them
        /// </summary>
        [Header("Base UI")]
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text bodyText;
        [SerializeField] protected Image rarityImage;
        [SerializeField] protected Image artworkImage;
        [SerializeField, Tooltip("Used when the artwork is not found")] protected Sprite placeholderArtwork;
        [SerializeField] protected Image artworkMask;
        [SerializeField] protected ARarityView rarityView;
        [SerializeField] protected UnityDictionary<CardTypeSettings> settingByCardTypes;
        [SerializeField] protected UnityDictionary<StatView> statsById;

        [Header("Card Highlight")]
        [SerializeField] protected GameObject cardHighlightParent;
        [SerializeField] protected Image cardHighlight;
        [SerializeField] protected Toggle toggle;

        public Action<UserInput> OnUserInput { get; set; }

        #region ICardView

        // private void Start()
        // {
        //     toggle.group = gameObject.GetComponentInParent<ToggleGroup>();
        //     toggle.onValueChanged.AddListener(value=>ToggleHighlight(value, Color.white));
        // }

        public void SetStat(string id, int originalValue, int value, int statMaxValue)
        {
            UpdateStat(id, originalValue, value);
        }

        public void SetCardName(string name)
        {
            nameText.text = name;
            this.name = $"{name}";
        }

        public void SetCardBodyText(string text)
        {
        }

        public virtual void SetCardType(string cardType, string incandescense)
        {
            var cardTypeSetting = settingByCardTypes.GetItem(cardType);

            //artworkMask.sprite = cardTypeSetting.artworkMask;

            foreach (var objItem in cardTypeSetting.objectsToActivate)
            {
                objItem.obj.SetActive(objItem.activated);
            }
        }

        public void SetArtwork(Sprite sprite)
        {
            artworkImage.sprite = sprite ?? placeholderArtwork;
        }

        public void SetRarity(string rarity)
        {
            rarityView.SetRarity(rarity);
        }

        public void SetWorld(string worldId)
        {
        }

        public void SetOnDestroy(Action onDestroy)
        {
        }

        public void Destroy()
        {
            if (gameObject != null)
                Destroy(gameObject);
        }

        public void ToggleHighlight(bool enabled, Color color = default, bool showHandHighlight = true)
        {
            cardHighlightParent.SetActive(enabled);
            cardHighlight.gameObject.SetActive(enabled);
            cardHighlight.color = color;
        }

        public void SetKeywordList(bool enabled, List<string> keywords, List<string> effects, List<string> createdCards = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected void UpdateStat(string id, int originalValue, int value)
        {
            var statView = statsById.GetItem(id);

            if (statView != null)
            {
                statView.SetStat(originalValue, value);
            }
            else
            {
                Debug.LogWarning($"There is no view to show the stat {id}. Do you need to implement one?");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnUserInput?.Invoke(UserInput.HoverStarted);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnUserInput?.Invoke(UserInput.HoverEnded);
        }

        [Serializable]
        protected class CardTypeSettings
        {
            [Tooltip("Objects activated when this setting is selected")]
            public List<ObjectToToggle> objectsToActivate;
        }

        [Serializable]
        protected class ObjectToToggle
        {
            public GameObject obj;
            public bool activated;
        }
    }
}