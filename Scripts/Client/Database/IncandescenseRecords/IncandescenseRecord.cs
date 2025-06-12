using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Database
{
    [CreateAssetMenu(fileName = "new Incandescense", menuName = "Dawnshard/Incandescense", order = -1)]
    public class IncandescenseRecord : ScriptableObject
    {
        public string incandescenseId;
        public Sprite deckFrame;
        public Sprite cardSetFrame;

        
        [Header("Card Types Settings")]
        public Sprite actionFrame;
        public Sprite artifactFrame;
        public Sprite creatureHorizontalFrame;
        public Sprite creatureTopFrame;
        public Sprite creatureBottomFrame;
        public Sprite creatureDivider;
        public Sprite worldFrame;
    }
}
