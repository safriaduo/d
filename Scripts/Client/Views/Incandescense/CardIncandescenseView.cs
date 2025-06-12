using Dawnshard.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    public class CardIncandescenseView : AIncandescenseView
    {
        [SerializeField] private SpriteRenderer defaultFrame;
        [SerializeField] private SpriteRenderer horizontalFrame;
        [SerializeField] private SpriteRenderer topFrame;
        [SerializeField] private SpriteRenderer bottomFrame;
        [SerializeField] private SpriteRenderer divider;
        [SerializeField] private SpriteRenderer worldFrame;

        private string cardType;

        public void SetIncandescense(string incandescenseId, string cardType)
        {
            this.cardType = cardType;
            SetIncandescense(incandescenseId);
        }

        protected override void UpdateView(IncandescenseRecord incandescenseRecord)
        {

            if (cardType == Constants.ActionType)
            {
                ToggleCreatureFrames(false);
                defaultFrame.sprite = incandescenseRecord.actionFrame;
            }
            else if (cardType == Constants.ArtifactType)
            {
                ToggleCreatureFrames(false);
                defaultFrame.sprite = incandescenseRecord.artifactFrame;
            }
            else if (cardType == Constants.CreatureType)
            {
                ToggleCreatureFrames(true);
                worldFrame.sprite = incandescenseRecord.worldFrame;
                horizontalFrame.sprite = incandescenseRecord.creatureHorizontalFrame;
                topFrame.sprite = incandescenseRecord.creatureTopFrame;
                bottomFrame.sprite = incandescenseRecord.creatureBottomFrame;
                divider.sprite = incandescenseRecord.creatureDivider;
            }
        }

        private void ToggleCreatureFrames(bool enabled)
        {
            defaultFrame.gameObject.SetActive(!enabled);
            horizontalFrame.gameObject.SetActive(enabled);
            topFrame.gameObject.SetActive(enabled);
            bottomFrame.gameObject.SetActive(enabled);
            divider.gameObject.SetActive(enabled);
            worldFrame.gameObject.SetActive(enabled);
        }
    }
}