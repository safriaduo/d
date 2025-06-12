using Dawnshard.Network;

namespace Dawnshard.Presenters
{
    public class LogPresenter : ILogPresenter
    {
        public LogModel LogModel { get; set; }
        protected readonly ILogView logView;

        public LogPresenter(ILogView logView, LogModel logModel)
        {
            LogModel = logModel;
            this.logView = logView;
            UpdateView();
        }

        public virtual void UpdateView()
        {
            logView.SetIcon(LogModel.ActionIcon, LogModel.ActionText, LogModel.LocalPlayer);
            logView.SetSourceCard(LogModel.SourceCard);
            logView.SetTargets(LogModel.CardTarget);
        }

        public void DestroyView()
        {
            logView.DestroyView();
        }
    }
}
