using Dawnshard.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    public abstract class AIncandescenseView : MonoBehaviour
    {
        public void SetIncandescense(string incandescenseId)
        {
            UpdateView(AssetDatabase.Instance.GetIncandescenseRecord(incandescenseId));
        }

        protected abstract void UpdateView(IncandescenseRecord incandescenseRecord);
    }
}
