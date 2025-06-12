using Dawnshard.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views {
    public class OpponentBoardZoneView : BoardZoneView {
        protected override void AddCard(ICardPresenter cardPresenter) {
            base.AddCard(cardPresenter);
            ShowPlayInfoCard(cardPresenter.Model);
            StartCoroutine(HideAfterAWhile());
        }

        private IEnumerator HideAfterAWhile() {
            yield return new WaitForSeconds(2);
            HidePlayInfoCard();
        }
    }
}