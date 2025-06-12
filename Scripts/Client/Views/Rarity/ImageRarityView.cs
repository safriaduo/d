using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class ImageRarityView : ARarityView
    {

        [SerializeField] private Image rarityRenderer;

        protected override void UpdateView(RarityRecord rarityRecord)
        {
            rarityRenderer.color = rarityRecord.mainColor;
        }
        
    }
}
