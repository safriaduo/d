using System;
using Dawnshard.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class RankedMatchPopup : MatchPopup
    {
        [SerializeField] private Image rankImage = null;
        [SerializeField] private Image rankExp = null;
        [SerializeField] private GameObject rankExpBar = null;
        [SerializeField] private TMP_Text rankNumber = null;
        [SerializeField] private TMP_FontAsset bebasAsset;

        public bool IsReverseMode { get; set; } = false;

        protected override void StartMatchAsync()
        {
            FindMatchAsync();
        }

        private async void FindMatchAsync()
        {
            try
            {
                findMatchButton.interactable = false;
                closeButton.interactable = false;
                timerText.text = "00:00";

                await GameController.Instance.FindMatch(deckPresenter.Model.Name,
                    IsReverseMode ? Constants.ReverseMode : Constants.RankedMode);

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
            }
        }

        protected override async void CancelMatchAsync()
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
            base.Open();
            if (rankExp is null || rankImage is null) return;
            SetRankImage();
            if (IsReverseMode)
                titleText.text = Constants.ReverseMode + " Match";
            else
                titleText.text = Constants.RankedMode + " Match";
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
                    .Levels[RankAPI.RankEntry.CurrentLevel - 1]
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
    }
}
