using Dawnshard.Network;
using Dawnshard.Presenters;
using UnityEngine;

namespace Dawnshard.Network {
    public abstract class EventCatcher : MonoBehaviour {
        protected IGamePresenter gamePresenter;


        protected virtual void Start()
        {
            GameController.Instance.EventDispatcher.RegisterEventCatcher(this);    
        }

        public virtual void SetGamePresenter(IGamePresenter gamePresenter) {
            this.gamePresenter = gamePresenter;
        }

        public abstract void OnGameUpdate(EventDispatcher.GameEventType gameEvent, GameUpdate gameUpdate);
    }
}