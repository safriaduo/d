using Dawnshard.Menu;
using Dawnshard.Network;
using System;
using System.Collections;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Dawnshard.Views;
using Dawnshard.Presenters;

namespace Dawnshard.Menu
{
    public abstract class MatchPopup : Popup
    {
        [SerializeField] protected Button findMatchButton;
        [SerializeField] protected MMFeedbacks openAnimation;
        [SerializeField] protected MMFeedbacks waitingAnimation;
        [SerializeField] protected Transform closePosition;
        [SerializeField] protected GameObject deckParent;
        [SerializeField] protected TMP_Text timerText;
        [SerializeField] protected TMP_Text titleText;

        protected DeckPresenter deckPresenter;
        private bool isOpen = false;
        public bool isSearchingForMatch = false;
        private float timer;

        protected Action OnPopupClosed;

        protected override void Start()
        {
            timer = 0f;
            base.Start();
            if (findMatchButton != null)
                findMatchButton.onClick.AddListener(ManageSearch);
        }

        private void ManageSearch()
        {
            if (!isSearchingForMatch)
            {
                StartMatchAsync();
            }
            else
            {
                CancelMatchAsync();
            }
        }

        private void Update()
        {
            if (isSearchingForMatch)
            {
                timer += Time.deltaTime;
                float minutes = Mathf.FloorToInt(timer / 60);
                float seconds = Mathf.FloorToInt(timer % 60);
                if (timerText != null)
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                timer = 0f;
            }
        }

        public virtual void SetDeckView(DeckModel deck, Action OnEnd)
        {
            if (deckPresenter == null)
            {
                deckPresenter = DeckFactory.Instance.CreateDeckView(deck, deckParent.transform);
            }
            else
            {
                deckPresenter.Model = deck;
                deckPresenter.UpdateView();
            }
            OnPopupClosed = OnEnd;
        }

        protected abstract void StartMatchAsync();

        protected virtual async void CancelMatchAsync() { }

        public override void Open()
        {
            if (isOpen)
                return;
            base.Open();
            openAnimation.PlayFeedbacks();
            isOpen = true;
            timer = 0f;
        }

        public void SetOnCloseAction(Action OnClose)
        {
            OnPopupClosed += OnClose;
        }

        public override void Close()
        {
            if (!isOpen)
                return;
            StartCoroutine(CloseCoroutine());
        }

        public void CloseInstantly()
        {
            if (!isOpen)
                return;
            GetComponent<RectTransform>().position = closePosition.GetComponent<RectTransform>().position;
            isOpen = false;
            base.Close();
            OnPopupClosed?.Invoke();
        }

        private IEnumerator CloseCoroutine()
        {
            openAnimation.PlayFeedbacks();
            yield return new WaitWhile(() => openAnimation.IsPlaying || isSearchingForMatch);
            isOpen = false;
            OnPopupClosed?.Invoke();
            base.Close();
        }

        protected void UndoWaitingAnimation()
        {
            if (!isSearchingForMatch)
                return;
            waitingAnimation.PlayFeedbacks();
        }

        protected void StopWaitingAnimation()
        {
            waitingAnimation.StopFeedbacks();
        }

        protected void PlayWaitingAnimation()
        {
            if (isSearchingForMatch)
                return;
            waitingAnimation.PlayFeedbacks();
        }
    }
}
