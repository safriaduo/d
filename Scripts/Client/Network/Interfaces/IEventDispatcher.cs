using Dawnshard.Presenters;
using System.Collections.Generic;

namespace Dawnshard.Network
{
    public interface IEventDispatcher
    {
        /// <summary>
        /// The game started
        /// </summary>
        void OnGameStart(GameStart response);

        void RegisterGamePresenter(IGamePresenter presenter);
        void RegisterEventCatcher(EventCatcher eventCatcher);

        /// <summary>
        /// The game state has been updated
        /// </summary>
        /// <param name="response"></param>
        void OnGameStateUpdate(GameStateUpdate response);

        /// <summary>
        /// The game ended
        /// </summary>
        void OnGameEnd(GameEnd response);

        /// <summary>
        /// The opponent left the match
        /// </summary>
        /// <param name="json"></param>
        void OnOpponentLeft(GameEnd json);

        /// <summary>
        /// A player took an action
        /// </summary>
        void OnGameUpdate(GameUpdate response);
    }
}