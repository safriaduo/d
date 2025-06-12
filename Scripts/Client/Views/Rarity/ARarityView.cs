using Dawnshard.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Views
{
    public abstract class ARarityView : MonoBehaviour
    {
        public void SetRarity(string rarityId)
        {
            UpdateView(AssetDatabase.Instance.GetRarityRecord(rarityId));
        }

        protected abstract void UpdateView(RarityRecord rarityRecord);
    }
}
