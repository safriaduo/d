using Dawnshard.Menu;
using Dawnshard.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dawnshard.Database;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Dawnshard.Views;
using Dawnshard.Presenters;

namespace Dawnshard.Menu
{
    public class RankedMatchPopup : Popup
    {
        [SerializeField] protected Button findMatchButton;
        [SerializeField] private MMFeedbacks openAnimation;
        [SerializeField] private MMFeedbacks waitingAnimation;
        [SerializeField] private Transform closePosition;
        [SerializeField] private GameObject deckParent;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image rankImage=null;
        [SerializeField] private Image rankExp=null;
        [SerializeField] private GameObject rankExpBar=null;
        [SerializeField] private TMP_Text rankNumber=null;
        [SerializeField] private TMP_FontAsset bebasAsset;


        protected DeckPresenter deckPresenter;
        private bool isOpen = false;
        public bool isSearchingForMatch = false;
        private float timer;

        public bool IsSinglePlayer { get; set; } = false;
        public bool IsReverseMode { get; set; } = false;

        protected Action OnPopupClosed;

        protected override void Start()
        {
            timer = 0f;
            base.Start();
            findMatchButton.onClick.AddListener(ManageSearch);
        }

        private void ManageSearch()
        {
            if (!isSearchingForMatch)
            {
                if (IsSinglePlayer)
                    StartAIMatchAsync();
                else
                    FindMatchAsync();
            }
            else
            {
                CancelMatchAsync();
            }
            //findMatchButton.interactable = false;
            //findMatchButton.interactable = true;
        }

        private void Update()
        {
            if (isSearchingForMatch)
            {
                timer += Time.deltaTime;
                float minutes = Mathf.FloorToInt(timer / 60);
                float seconds = Mathf.FloorToInt(timer % 60);
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

        protected virtual async void StartAIMatchAsync()
        {
            PlayWaitingAnimation();

            try
            {
                await GameController.Instance.StartAIMatch(deckPresenter.Model.Name,false);
                isSearchingForMatch = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowError(e.Message);
                return;
            }
        }

        public async void FindMatchAsync()
        {
            try
            {
                findMatchButton.interactable = false;
                closeButton.interactable = false;
                timer = 0f;
                float minutes = Mathf.FloorToInt(timer / 60);
                float seconds = Mathf.FloorToInt(timer % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                await GameController.Instance.FindMatch(deckPresenter.Model.Name, IsReverseMode? Constants.ReverseMode: Constants.RankedMode);
                waitingAnimation.Events.OnComplete.AddListener(() =>
                {
                    isSearchingForMatch = true;
                    findMatchButton.interactable = true;
                    closeButton.interactable = true;

                    waitingAnimation.Events.OnComplete.RemoveAllListeners();
                });
                PlayWaitingAnimation();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowError(e.Message);
                return;
            }
        }

        public async void CancelMatchAsync()
        {
            if (!isSearchingForMatch)
                return;
            try
            {
                findMatchButton.interactable = false;
                await GameController.Instance.CancelMatch();
                waitingAnimation.Events.OnComplete.AddListener(() =>
                {
                    isSearchingForMatch = false;
                    findMatchButton.interactable = true;
                    waitingAnimation.Events.OnComplete.RemoveAllListeners();
                });
                UndoWaitingAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void Open()
        {
            if (isOpen)
                return;
            base.Open();
            openAnimation.PlayFeedbacks();
            isOpen = true;
            if(rankExp is null || rankImage is null) return;
            SetRankImage();
            timer = 0f;
            if (IsReverseMode)
            {
                titleText.text = Constants.ReverseMode + " Match";
            }
            else
            {
                titleText.text = Constants.RankedMode + " Match";
            }
        }

        private void SetRankImage()
        {
            if (IsReverseMode)
            {
                rankImage.gameObject.SetActive(false);
                rankExpBar.SetActive(false);
                rankExp.gameObject.SetActive(false);
                rankNumber.gameObject.SetActive(false);
                return;
            }
            rankImage.gameObject.SetActive(true);
            rankExpBar.SetActive(true);
            rankExp.gameObject.SetActive(true);
            rankNumber.gameObject.SetActive(true);
            if (RankAPI.RankEntry != null)
            {
                if (RankAPI.IsPlayerAscendantRank)
                {
                    rankImage.sprite = AssetDatabase.Instance.GetRankRecord(Constants.AscendantRank).rankSprite;
                    rankNumber.text = RankAPI.RankPosition.ToString();
                    rankExpBar.SetActive(false);
                    rankExp.gameObject.SetActive(false);
                    return;
                }
                rankImage.sprite = AssetDatabase.Instance.GetRankRecord(RankAPI.RankEntry.CurrentRank).rankSprite;
                rankExp.fillAmount = (float)RankAPI.RankEntry.SubScore / RankAPI.Ranks
                    .Find(rank => rank.Name == RankAPI.RankEntry.CurrentRank)
                    .Levels[RankAPI.RankEntry.CurrentLevel-1]
                    .ExpToLevelUp;
                    var currentRankLevelName = " ";
                    for (var i = 0; i < RankAPI.RankEntry.CurrentLevel; i++)
                        currentRankLevelName += "I";
                    rankNumber.text = RankAPI.RankEntry.CurrentRank + currentRankLevelName;
                    rankNumber.font = bebasAsset;
            }
            else
            {
                rankImage.sprite = AssetDatabase.Instance.GetRankRecord(Constants.WandererRank).rankSprite;
                rankExp.fillAmount = 0;
                rankNumber.gameObject.SetActive(false);
            }
        }

        public void SetOnCloseAction(Action OnClose)
        {
            OnPopupClosed += OnClose;
        }

        public override void Close()
        {
            if (!isOpen)
                return;
            if (!IsSinglePlayer)
                CancelMatchAsync();
            StartCoroutine(CloseCoroutine());
        }

        public void CloseInstantly()
        {
            if (!isOpen)
                return;
            GetComponent<RectTransform>().position = closePosition.GetComponent<RectTransform>().position;
            isOpen = false;
            base.Close();
            OnPopupClosed();
        }

        private IEnumerator CloseCoroutine()
        {
            openAnimation.PlayFeedbacks();
            yield return new WaitWhile(() => openAnimation.IsPlaying || isSearchingForMatch);
            isOpen = false;
            OnPopupClosed?.Invoke();
            base.Close();
        }

        private void OnMatchFound()
        {
            StopWaitingAnimation();

            var op = SceneManager.LoadSceneAsync(Constants.GameScene);
            op.allowSceneActivation = false;

            void onComplete()
            {
                op.allowSceneActivation = true;
            }

            PlayStartMatchAnimation(onComplete);
        }


        private void PlayStartMatchAnimation(Action OnCompleted)
        {
            OnCompleted?.Invoke();
        }

        private void UndoWaitingAnimation()
        {
            if (!isSearchingForMatch)
                return;
            waitingAnimation.PlayFeedbacks();
        }

        private void StopWaitingAnimation()
        {
            waitingAnimation.StopFeedbacks();
        }

        private void PlayWaitingAnimation()
        {
            if (isSearchingForMatch)
                return;
            waitingAnimation.PlayFeedbacks();
        }
    }
}
