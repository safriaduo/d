using Dawnshard.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a world in UI
    /// </summary>
    public abstract class AWorldView : MonoBehaviour
    {
        public void SetWorld(string worldId)
        {
            UpdateView(AssetDatabase.Instance.GetWorldRecord(worldId));
        }

        protected abstract void UpdateView(WorldRecord worldRecord);
    }
}