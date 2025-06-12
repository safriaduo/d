using System.Collections;
using UnityEngine;

namespace Dawnshard.Network
{
    /// <summary>
    /// This component has the responsability to compose the server request and call the network gateway
    /// to send the messages
    /// </summary>
    public class GameInteractor : IGameInteractor
    {
        private readonly INetworkGateway networkGateway;

        private static bool IsLocked { get; set; }

        public GameInteractor(INetworkGateway networkGateway)
        {
            this.networkGateway = networkGateway;
            IsLocked = false;
        }

        #region IGameInteractor
        public void DiscardCard(CardModel cardModel)
        {
            if (IsLocked) { return; }

            DiscardCardRequest request = new()
            {
                CardId = cardModel.InstanceId
            };

            networkGateway.SendDiscardCardRequestAsync(request);
        }

        public void FightCreature(CardModel cardModel, int defenderInstanceId)
        {
            if (IsLocked) { return; }

            FightCreatureRequest request = new()
            {
                AttackerId = cardModel.InstanceId,
                DefenderId = defenderInstanceId,
            };

            networkGateway.SendFightCreatureRequestAsync(request);
        }

        public void PlayCard(CardModel cardModel, int onPlay = -1, int replace = -1)
        {
            if (IsLocked) { return; }

            PlayCardRequest request = new()
            {
                CardId = cardModel.InstanceId,
            };

            if (onPlay >= 0 || replace >= 0)
            {
                request.CardTargetId = onPlay;
                request.ReplacedCardId = replace;
            }

            networkGateway.SendPlayCardRequestAsync(request);
        }

        public void ReapCreature(CardModel cardModel)
        {
            if (IsLocked) { return; }

            ReapCreatureRequest request = new()
            {
                CardId = cardModel.InstanceId
            };

            networkGateway.SendReapCreatureRequestAsync(request);
        }

        public void SelectActiveWorld(string worldId)
        {
            if (IsLocked) { return; }
            SelectActiveWorldRequest request = new()
            {
                WorldId = worldId
            };

            networkGateway.SendSelectActiveWorldRequestAsync(request);
        }

        public void AcceptMulligan(bool accept)
        {
            if (IsLocked) { return; }

            MulliganRequest request = new()
            {
                Accepted = accept
            };
            networkGateway.SendMulliganWorldRequestAsync(request);
        }

        public void StopTurn()
        {
            if (IsLocked) { return; }

            StopTurnRequest request = new();

            networkGateway.SendStopTurnRequestAsync(request);
        }

        #endregion

        public static void Lock(bool enabled)
        {
            IsLocked = enabled;
        }
    }
}
