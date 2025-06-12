using UnityEngine;

namespace Dawnshard.Database
{
    [CreateAssetMenu(fileName = "new World", menuName = "Dawnshard/WorldRecord", order = -1)]
    public class WorldRecord : ScriptableObject
    {
        public string worldId;
        public Sprite worldIcon;
        public Color mainColor;
        public Color backgroundColor;
        public Sprite bodyBackground;
        public Material worldButtonMaterial;
        public GameObject worldBoard;
    }
}