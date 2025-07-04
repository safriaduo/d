using System;
using UnityEngine;
using Dawnshard.Network;

namespace Dawnshard.Menu
{
    /// <summary>
    /// Popup to join a friendly match using an existing match id.
    /// Behaviour is similar to RankedMatchPopup but instead of finding a match
    /// it directly joins the provided match.
    /// </summary>
    public class FriendlyMatchPopup : RankedMatchPopup
    {
        private string matchId;

        /// <summary>
        /// Set the match id to join when the popup starts the match.
        /// </summary>
        /// <param name="matchId">Match identifier provided by the inviter.</param>
        public void SetMatch(string matchId)
        {
            this.matchId = matchId;
        }

        /// <inheritdoc />
        protected override async void StartAIMatchAsync()
        {
            //PlayWaitingAnimation();
            try
            {
                await GameController.Instance.JoinFriendlyMatch(matchId, deckPresenter.Model.Name);
                FriendlyMatchManager.Clear();
                isSearchingForMatch = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowError(e.Message);
            }
        }
    }
}
