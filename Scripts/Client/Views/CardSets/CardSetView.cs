using Dawnshard.Network;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AssetDatabase = Dawnshard.Database.AssetDatabase;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a player card set
    /// </summary>
    public class CardSetView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text artifactText;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text creatureText;
        [SerializeField] private AWorldView worldView;
        [SerializeField] private Image artworkImage;
        [SerializeField] private AIncandescenseView incandescenseImage;
        [SerializeField] private AIncandescenseView incandescenseImageHighlight;
        [SerializeField] private Transform listParent;
        [SerializeField] private BaseCardView infoCardView;
        [SerializeField] private GameObject highlight;
        [SerializeField] private MMFeedbacks addCardAnimation;
        [SerializeField] private Image setBackground;
        [SerializeField] private Image setBorder;
        [SerializeField] private Button infoButton;



        private IBaseCardPresenter infoPresenter;
        public Vector3 offset; // Imposta l'offset desiderato nell'Inspector
        public bool isVisible = true;
        [SerializeField] protected MMFeedbacks showAnimation;
        
        
        private void Awake()
        {
            if (infoButton != null)
            {
                infoButton.gameObject.SetActive(false);
            }
        }
        
        void Update()
        {
            if (infoCardView.isActiveAndEnabled)
            {

                // Ottieni la posizione del puntatore del mouse
                Vector3 mousePosition = Input.mousePosition;

                // Converti la posizione del mouse dalla coordinata dello schermo a quella del mondo
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                // Aggiungi l'offset
                infoCardView.transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
            }
        }

        public void ShowCardHighlight(CardModel model)
        {
            if (infoPresenter == null)
            {
                infoPresenter = new BaseCardPresenter(infoCardView, model);
            }
            else
            {
                infoPresenter.Model = model;
                infoPresenter.SetStaticFields();
            }

            infoCardView.gameObject.SetActive(true);
        }

        public void HideCardHighlight()
        {
            infoCardView.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set the name of a deck
        /// </summary>
        public void SetName(string deckName)
        {
            nameText.text = deckName;
        }

        public void SetCardTypeNumber(int creature, int artifact, int action)
        {
            creatureText.text = creature.ToString();
            artifactText.text = artifact.ToString();
            actionText.text = action.ToString();
        }

        /// <summary>
        /// Sets the world of a deck
        /// </summary>
        public void SetWorld(string worldId)
        {
            worldView.SetWorld(worldId);
            if(setBackground!= null)
                setBackground.color = AssetDatabase.Instance.GetWorldRecord(worldId).backgroundColor;
            if(setBorder!=null)
                setBorder.color = AssetDatabase.Instance.GetWorldRecord(worldId).mainColor;
        }

        public void SetArtwork(Sprite sprite)
        {
            artworkImage.sprite = sprite;
        }

        public void SetIncandescense(string incandescense)
        {
            incandescenseImage.SetIncandescense(incandescense);
            incandescenseImageHighlight.SetIncandescense(incandescense);
        }

        public Transform GetCardListParent()
        {
            return listParent;
        }

        public void Destroy()
        {
            Destroy((gameObject));
        }

        public void ShowSet(bool enable)
        {
            gameObject.SetActive(enable);
        }
        

        public void ShowNextCard(bool enable, int cardShown)
        {
            int i = 0;
            foreach (Transform child in listParent)
            {
                if (child.gameObject.activeSelf!=enable)
                {
                    if (enable)
                    {
                        StartCoroutine(SetActiveDelayCoroutine(child.gameObject, true));
                        addCardAnimation.PlayFeedbacks();
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                    if(i >= cardShown)
                        return;
                }
                i++;
            }
        }

        private IEnumerator SetActiveDelayCoroutine(GameObject target, bool enable)
        {
            yield return new WaitForSeconds(0.2f);
            target.SetActive(enable);
        }
        
        public virtual IEnumerator ToggleVisibilityAnimation(bool show, Action OnEndAction = null)
        {
            if (!isVisible && show)
            {
                isVisible = true;
                showAnimation.Direction = MMFeedbacks.Directions.TopToBottom;
                showAnimation.PlayFeedbacks();
                yield return new WaitUntil(() => !showAnimation.IsPlaying);
                OnEndAction?.Invoke();
                GetComponent<CanvasGroup>().alpha = 1;
            }
            else if(isVisible && !show)
            {
                isVisible = false;
                showAnimation.Direction = MMFeedbacks.Directions.BottomToTop;
                showAnimation.PlayFeedbacks();
                yield return new WaitUntil(() => !showAnimation.IsPlaying);
                OnEndAction?.Invoke();
            }
        }
        
        public void AddButtonInfoListener(Action OnClickAction)
        {
            if(infoButton==null)
                return;
            infoButton.gameObject.SetActive(true);
            infoButton.onClick.AddListener(() => OnClickAction());
        }

        public void RemoveButtonInfoListener(Action OnClickAction)
        {
            if(infoButton==null)
                return;
            infoButton.gameObject.SetActive(false);
            infoButton.onClick.RemoveListener(() => OnClickAction());
        }
    }
}
