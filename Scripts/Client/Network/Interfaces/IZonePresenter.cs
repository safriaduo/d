namespace Dawnshard.Network
{
    public interface IZonePresenter
    {
        /// <summary>
        /// Data model of a zone
        /// </summary>
        ZoneModel Model { get; set; }

        /// <summary>
        /// Refresh the view
        /// </summary>
        void UpdateView();

        /// <summary>
        /// Return true if the zone is interactable
        /// </summary>
        bool IsZoneInteractable();

        /// <summary>
        /// Update the models of the cards
        /// </summary>
        void UpdateCardsModels();
    }
}