using Dawnshard.Database;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    public class ModelWorldView : AWorldView
    {
        [SerializeField] private SpriteRenderer worldIcon;
        [SerializeField] private MeshRenderer backgroundModel;
        [SerializeField] private MMFeedbacks activateWorldAnimation;
        [SerializeField] private MMFeedbacks deactivateWorldAnimation;
        [SerializeField] private MMFeedbacks canBeActiveWorldAnimation;
        private WorldRecord worldRecord;
        //private Dictionary<string, WorldRecord> worldRecords=new();

        protected override void UpdateView(WorldRecord worldRecord)
        {
            worldIcon.sprite = worldRecord.worldIcon;
            worldIcon.color = worldRecord.mainColor;
            backgroundModel.material.color = worldRecord.backgroundColor;
            this.worldRecord = worldRecord;
        }

        /// <summary>
        /// Play the world deactivation animation
        /// </summary>
        public void DeactivateWorld(Material deactivateMaterial) {
            deactivateWorldAnimation.GetComponent<MMFeedbackMaterial>().Materials.Add(deactivateMaterial);
            deactivateWorldAnimation.PlayFeedbacks();
        }

        /// <summary>
        /// Play the world activation animation
        /// </summary>
        public void ActivateWorld(string worldID) {
            SetWorld(worldID);
            activateWorldAnimation.GetComponent<MMFeedbackMaterial>().Materials.Add(worldRecord.worldButtonMaterial);
            worldIcon.color = AssetDatabase.Instance.GetWorldRecord(worldID).mainColor;
            activateWorldAnimation.PlayFeedbacks();
        }

        /// <summary>
        /// Play the world toggle animation
        /// </summary>
        public void ToggleSelectionWorld(Material canBeActiveMaterial) {
            canBeActiveWorldAnimation.GetComponent<MMFeedbackMaterial>().Materials.Add(canBeActiveMaterial);
            canBeActiveWorldAnimation.PlayFeedbacks();
        }

        //public WorldRecord GetRecord() {
        //    return worldRecord;
        //}
    }
}
