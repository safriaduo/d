using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public interface IGamePresenter
    {

        GameStateModel Model { get; set; }

        /// <summary>
        /// Update all the models
        /// </summary>
        void UpdateModel();
        
        /// <summary>
        /// Update all the views so that they match with the models
        /// </summary>
        void UpdateView(bool updateAllPlayers);
        
        /// <summary>
        /// Update all the models for the combat log
        /// </summary>
        void UpdateLogModels();

        /// <summary>
        /// Initialize the presenter
        /// </summary>
        void Initialize(EventBusManager eventBusManager);
        
        /// <summary>
        /// Get the player with the corresponding id
        /// </summary>
        IPlayerPresenter GetPlayer(string id);

        /// <summary>
        /// Get both players
        /// </summary>
        List<IPlayerPresenter> GetBothPlayers();

        /// <summary>
        /// Method that is called when the EventDispatcher ends the elaboration of a Response
        /// </summary>
        void ResponseElaborationEnded();
    }
}