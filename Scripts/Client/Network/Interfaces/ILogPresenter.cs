using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Network
{
    public interface ILogPresenter
    {
        LogModel LogModel { get; set; }

        void UpdateView();

        void DestroyView();
    }
}