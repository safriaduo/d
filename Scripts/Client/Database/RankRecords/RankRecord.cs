
using UnityEngine;

namespace Dawnshard.Database
{
    [CreateAssetMenu(fileName = "new Rank", menuName = "Dawnshard/Rank", order = -1)]

    public class RankRecord : ScriptableObject
    {
        public string rankID;
        public Color rankColor;
        public Sprite rankSprite;
    }
}
