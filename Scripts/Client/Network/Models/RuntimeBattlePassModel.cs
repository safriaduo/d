using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network{
    public class RuntimeBattlePassModel
    {

        /// <summary>
        /// Name of the battlepass referring to
        /// </summary>
        public string battlePassName;

        /// <summary>
        /// Battle pass last redeemed level. Goes from -1 to 50
        /// </summary>
        public int currentLevel;

        /// <summary>
        /// Battle pass last redeemed level
        /// </summary>
        public int currentExp;

        /// <summary>
        /// Battle pass premium unlocked
        /// </summary>
        public bool premiumUnlocked;
    }

}