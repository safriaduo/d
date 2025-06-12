
using UnityEngine;

namespace Dawnshard.Database
{
    [CreateAssetMenu(fileName = "new Rarity", menuName = "Dawnshard/Rarity", order = -1)]
    public class RarityRecord : ScriptableObject
    {
        public string rarityId;
        public Color mainColor;
        public Sprite raritySprite;
    }
}