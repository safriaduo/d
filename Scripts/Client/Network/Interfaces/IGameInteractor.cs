namespace Dawnshard.Network
{
    public interface IGameInteractor
    {
        void DiscardCard(CardModel cardModel);
        void FightCreature(CardModel cardModel, int defenderId);
        void PlayCard(CardModel cardModel, int onPlay = -1, int replace = -1);
        void ReapCreature(CardModel cardModel);
        void SelectActiveWorld(string world);
        void AcceptMulligan(bool accept);
        void StopTurn();
    }
}