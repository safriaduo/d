using Dawnshard.Database;
using UnityEngine;

namespace Dawnshard.Views
{
    public class SpriteRarityView : ARarityView
    {
        [SerializeField] private SpriteRenderer rarityRenderer;

        protected override void UpdateView(RarityRecord rarityRecord)
        {
            rarityRenderer.sprite = rarityRecord.raritySprite;
        }
    }
}
