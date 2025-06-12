using System.Collections;
using UnityEngine;

namespace Dawnshard.Network
{
    /// <summary>
    /// This component has the responsability to compose the server request and call the network gateway
    /// to send the messages
    /// </summary>
    public class TutorialGameInteractor : IGameInteractor
    {
        private readonly INetworkGateway networkGateway;

        private static bool IsLocked { get; set; }
        
        public TutorialGameInteractor(INetworkGateway networkGateway)
        {
            this.networkGateway = networkGateway;
            IsLocked = false;
        }
        
        #region IGameInteractor
        public void DiscardCard(CardModel cardModel)
        {
            if (IsLocked || 
                TutorialManager.currentTutorialStep < TutorialManager.TutorialSteps.WaitingForDiscard ) { return; }

            DiscardCardRequest request = new()
            {
                CardId = cardModel.InstanceId
            };

            networkGateway.SendDiscardCardRequestAsync(request);
        }

        public void FightCreature(CardModel cardModel, int defenderInstanceId)
        {
            if (IsLocked ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Reap ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.HowToWin ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Action || 
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.World ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Artifact ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Discard) { return; }

            FightCreatureRequest request = new()
            {
                AttackerId = cardModel.InstanceId,
                DefenderId = defenderInstanceId,
            };

            networkGateway.SendFightCreatureRequestAsync(request);
        }

        public void PlayCard(CardModel cardModel, int onPlay = -1, int replace = -1)
        {
            if (IsLocked ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Discard || 
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Fight||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.HowToWin )
            {
                return;
            }

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
            if (IsLocked ||
                TutorialManager.currentTutorialStep < TutorialManager.TutorialSteps.Reap ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Action || 
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Artifact ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.World ||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.Discard||
                TutorialManager.currentTutorialStep == TutorialManager.TutorialSteps.HowToWin ) { return; }

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
