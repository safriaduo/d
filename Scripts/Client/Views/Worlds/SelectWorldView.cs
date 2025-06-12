using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// This view is the responsible to show the active world for
    /// the current player and select it if necessary
    /// </summary>
    public class SelectWorldView : MonoBehaviour
    {
        //[SerializeField] private ModelWorldView[] worldViews;
        [SerializeField] private ModelWorldView worldView;
        [SerializeField] private Material deactiveWorldMaterial;
        [SerializeField] private Material canBeActiveWorldMaterial;
        [SerializeField] private GameObject defaultBoard;
        //private Dictionary<string, int> idToInt = new();

        /// <summary>
        /// The active world can now be selected
        /// </summary>
        //public void ToggleSelection(Transform boardTransform)
        //{
        //    foreach (var worldView in worldViews)
        //    {
        //        worldView.ToggleSelectionWorld(canBeActiveWorldMaterial);
        //    }
        //    SubstituteBoard(boardTransform, defaultBoard);
        //}

        /// <summary>
        /// Activate or deactivate a specific world 
        /// </summary>

        //public void HighlightWorld(int worldId, string activeWorldId, Transform boardTransform)
        //{
        //    if (worldId == idToInt[activeWorldId])
        //    {
        //        worldViews[worldId].ActivateWorld();
        //        SubstituteBoard(boardTransform, worldViews[worldId].GetRecord().worldBoard);
        //    }
        //    else
        //    {
        //        worldViews[worldId].DeactivateWorld(deactiveWorldMaterial);
        //    }
        //}



        /// <summary>
        /// The active world was selected
        /// </summary>
        public void SetActiveWorld(string activeWorldId, bool isWorldSelected)
        {
            if (!isWorldSelected && String.IsNullOrEmpty(activeWorldId)) {
                //ToggleSelection(boardTransform);
                worldView.DeactivateWorld(deactiveWorldMaterial);
                return;
            }
            //for (int i = 0; i < worldViews.Length; i++) {
            worldView.ActivateWorld(activeWorldId);
            //}
        }

        /// <summary>
        /// Sets the selectable worlds 
        /// </summary>
        //public void SetWorlds(string[] worldIds)
        //{
        //    Debug.Log(worldIds.Length);
        //    for (int i = 0; i < worldIds.Length; i++)
        //    {
        //        worldViews[i].SetWorld(worldIds[i]);
        //        idToInt[worldIds[i]] = i;
        //    }
        //}

        /// <summary>
        /// Substitute the default board with the one of the selected world
        /// </summary>
        //public void SubstituteBoard(Transform boardTransform, GameObject newBoard) {
        //    foreach (Transform child in boardTransform) {
        //        if(child!=boardTransform)
        //            Destroy(child.gameObject);
        //    }
        //    Instantiate(newBoard, boardTransform);
        //}
    }
}