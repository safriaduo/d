using Dawnshard.Database;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a world in UI
    /// </summary>
    public class UIWorldView : AWorldView

    {
        [SerializeField] private Image worldIcon;
        [SerializeField] private Image background;

        protected override void UpdateView(WorldRecord worldRecord)
        {
            worldIcon.sprite = worldRecord.worldIcon;
            worldIcon.color = worldRecord.mainColor;
            background.color = worldRecord.backgroundColor;
        }
    }
}