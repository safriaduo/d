using Dawnshard.Database;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a world in a card
    /// </summary>
    public class CardWorldView : SpriteWorldView
    {
        [SerializeField] private SpriteRenderer bodyRenderer;

        protected override void UpdateView(WorldRecord worldRecord)
        {
            base.UpdateView(worldRecord);
            bodyRenderer.sprite = worldRecord.bodyBackground;

        }
    }
}