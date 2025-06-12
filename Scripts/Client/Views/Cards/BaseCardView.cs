using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dawnshard.Database;
using Dawnshard.Network;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// The card view shows the data of a card, exposing easy to access methods to modify them
    /// </summary>
    public class BaseCardView : MonoBehaviour, ICardView
    {
        [Header("Base UI")] [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text bodyText;
        [SerializeField] protected TMP_Text cardTypeText;
        [SerializeField] protected SpriteRenderer rarityImage;
        [SerializeField] protected SpriteRenderer artworkImage;

        [SerializeField, Tooltip("Used when the artwork is not found")]
        protected Sprite placeholderArtwork;

        [SerializeField] protected SpriteMask artworkMask;
        [SerializeField] protected CardWorldView worldView;
        [SerializeField] protected SpriteRarityView rarityView;
        [SerializeField] protected CardIncandescenseView incandescenseView;
        [SerializeField] protected UnityDictionary<CardTypeSettings> settingByCardTypes;
        [SerializeField] protected UnityDictionary<StatView> statsById;

        [Header("Card Highlight")] [SerializeField]
        protected GameObject cardHighlightParent;

        [SerializeField] protected SpriteRenderer handCardHighlight;
        [SerializeField] protected SpriteRenderer boardCardHighlight;
        [SerializeField] protected UnityDictionary<Sprite> cardTypeHighlight;

        [Header("Keyword Explanation")] [SerializeField]
        protected GameObject explanationsList;

        [SerializeField] protected GameObject explanationsParent;
        [SerializeField] protected UnityDictionary<GameObject> keywordExplanations;
        [SerializeField] protected UnityDictionary<GameObject> effectExplanations;
        [SerializeField] protected GameObject bloodboundExplanation;
        
        [SerializeField] private MMFeedbacks goToSetAnimation;

        [SerializeField] private MMFeedbackPosition goToSetPosition;


        public Action<UserInput> OnUserInput { get; set; }

        private Action OnDestroy { get; set; }
        private bool disableHighlight = false;

        #region ICardView

        public void SetStat(string id, int originalValue, int value, int maxValue)
        {
            UpdateStat(id, originalValue, value, maxValue);
        }

        public void SetCardName(string name)
        {
            nameText.text = name;
            this.name = $"{name}";
        }

        public void SetCardBodyText(string text)
        {
            var boldTextList = Constants.KeywordText.ToList();
            boldTextList.AddRange(Constants.TriggerText);
            var italicTextList = Constants.EffectsText.ToList();
            var formattedText = TextFormatter.BoldifyText(text, boldTextList);
            formattedText = TextFormatter.ItalicizeText(formattedText, italicTextList);
            formattedText = TextFormatter.ReplaceTextWithSpriteTags(formattedText, Constants.TextToIcon);

            bodyText.text = formattedText;
        }

        public virtual void SetCardType(string cardType, string incandescense)
        {
            cardTypeText.text = cardType;
            incandescenseView.SetIncandescense(incandescense, cardType);

            var cardTypeSetting = settingByCardTypes.GetItem(cardType);

            artworkMask.sprite = cardTypeSetting.artworkMask;

            foreach (var objItem in cardTypeSetting.objectsToActivate)
            {
                objItem.obj.SetActive(objItem.activated);
            }

            boardCardHighlight.sprite = cardTypeHighlight.GetItem(cardType);
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
            worldView.SetWorld(worldId);
        }

        public void Destroy()
        {
            if (gameObject != null)
            {
                gameObject.transform.position=100*Vector3.up;
                StartCoroutine(DestroyCoroutine());
            }
        }

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(2f);
            OnDestroy?.Invoke();
            Destroy(gameObject);
        }

        public void SetOnDestroy(Action onDestroy)
        {
            OnDestroy += onDestroy;
        }

        public void ToggleHighlight(bool enabled, Color color = default, bool showHandHighlight = true)
        {
            if (disableHighlight)
                return;
            try
            {
                cardHighlightParent.SetActive(enabled);
                boardCardHighlight.gameObject.SetActive(enabled);
                handCardHighlight.gameObject.SetActive(enabled && (showHandHighlight));
                handCardHighlight.color = color;
                boardCardHighlight.color = color;
            }
            catch
            {
                
            }
        }


        public void SetKeywordList(bool enabled, List<string> keywords, List<string> effects, List<string> createdCards=null)
        {
            if (!enabled || (keywords.Count == 0 && effects.Count == 0))
            {
                explanationsList.SetActive(false);
                return;
            }
            foreach (Transform child in explanationsParent.transform)
            {
                Destroy(child.gameObject);
            }
            if (bodyText.text.Contains(Constants.BloodboundText))
            {
                Instantiate(bloodboundExplanation, explanationsParent.transform);
            }
            foreach (string keyword in keywords)
            {
                if (keywordExplanations.GetItem(keyword) != null)
                    Instantiate(keywordExplanations.GetItem(keyword), explanationsParent.transform);
            }

            bool addKeywordFlag = false;
            List<string> wordKeyword = new();
            foreach (var effect in effects)
            {
                switch (effect)
                {
                    case Constants.AddKeywordEffect:
                    {
                        if (addKeywordFlag)
                            break;
                        addKeywordFlag = true;
                        foreach (string word in bodyText.text.Split(' ', '<', '>'))
                        {
                            if (keywordExplanations.GetItem(word) != null 
                                && !keywords.Contains(word, StringComparer.OrdinalIgnoreCase) 
                                && !wordKeyword.Contains(word, StringComparer.OrdinalIgnoreCase))
                            {
                                Instantiate(keywordExplanations.GetItem(word), explanationsParent.transform);
                                wordKeyword.Add(word);
                            }
                        }

                        break;
                    }
                    case Constants.CreateCardEffect:
                    {
                        //Debug.Log(Regex.Match(bodyText.text, @"(?<=\b[Cc]reate\s)\p{L}+").Value);
                        //string createdCardName = Regex.Match(bodyText.text, @"(?<=\bcreate \s+)\p{L}+").Value;
                        if(createdCards == null)
                            break;
                        foreach (var createdCard in createdCards)
                        {
                            if (createdCard == string.Empty)
                                break;
                            var createdCardModel = CardDatabase.GetCardByName(createdCard);
                            var createdCardExplanation = Instantiate(effectExplanations.GetItem(effect),
                                explanationsParent.transform);
                            var customExplanationView = createdCardExplanation.GetComponent<CustomExplanationView>();
                            customExplanationView.setTitle(createdCardModel.Name);
                            string explanationBodyText = createdCardModel.Type + "\n";
                            if (createdCardModel.Type == Constants.CreatureType)
                            {
                                explanationBodyText +=
                                    createdCardModel.Stats.Find(stat => stat.ID == Constants.AttackStat).Value +
                                    "/" + createdCardModel.Stats.Find(stat => stat.ID == Constants.HealthStat)
                                        .Value + "\n";
                            }

                            explanationBodyText += createdCardModel.BodyText;
                            customExplanationView.setBody(explanationBodyText);
                        }
                        break;
                    }
                    default:
                    {
                        if (effectExplanations.GetItem(effect) != null)
                        {
                            Instantiate(effectExplanations.GetItem(effect), explanationsParent.transform);
                        }

                        break;
                    }
                }
            }

            explanationsList.SetActive(true);
        }

        #endregion

        public void DisableHighlight()
        {
            disableHighlight = true;
        }

        protected void UpdateStat(string id, int originalValue, int value, int maxValue)
        {
            var statView = statsById.GetItem(id);

            if (statView != null)
            {
                statView.SetStat(originalValue, value, maxValue);
            }
            else
            {
                Debug.LogWarning($"There is no view to show the stat {id}. Do you need to implement one?");
            }
        }


        [Serializable]
        protected class CardTypeSettings
        {
            [Tooltip("The mask for the artwork")] public Sprite artworkMask;

            [Tooltip("Objects activated when this setting is selected")]
            public List<ObjectToToggle> objectsToActivate;
        }

        [Serializable]
        protected class ObjectToToggle
        {
            public GameObject obj;
            public bool activated;
        }
        
        public void SetSetPosition(Vector3 setPosition)
        {
            goToSetPosition.DestinationPosition = setPosition;
        }
        
        protected virtual void OnMouseDown()
        {
            OnUserInput?.Invoke(UserInput.MouseDown);
        }

        public void GoToSet()
        {
            goToSetAnimation.PlayFeedbacks();
        }
    }
}