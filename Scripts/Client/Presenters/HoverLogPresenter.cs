using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using UnityEngine;

namespace Dawnshard.Presenters
{
    public class HoverLogPresenter : LogPresenter
    {
        public HoverLogPresenter(ILogView logView, LogModel logModel) : base(logView, logModel) { }

        public override void UpdateView()
        {
            base.UpdateView();
            ILogPresenter newPresenter = null;
            logView.OnPointerEnter_LogEntry = () => newPresenter = LogFactory.Instance.CreateExtendedLog(LogModel) ;
            logView.OnPointerExit_LogEntry = () =>
            {
                if (newPresenter != null)
                    newPresenter.DestroyView();
            };
        }
    }
}
