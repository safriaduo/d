using Dawnshard.Database;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a world in a sprite
    /// </summary>
    public class SpriteWorldView : AWorldView
    {
        [SerializeField] private SpriteRenderer worldIcon;
        [SerializeField] private SpriteRenderer background;

        protected override void UpdateView(WorldRecord worldRecord)
        {
            worldIcon.sprite = worldRecord.worldIcon;
            worldIcon.color = worldRecord.mainColor;
            background.color = worldRecord.backgroundColor;
        }
    }
}