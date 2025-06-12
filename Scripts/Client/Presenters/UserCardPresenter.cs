using System.Collections;
using Dawnshard.Network;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Dawnshard.Presenters
{
    /// <summary>
    /// This presenter communicates the updates to the view and can send request made from the user to the game interactor
    /// </summary>
    public class UserCardPresenter : CardPresenter
    {
        protected IUserCardView UserCard => cardView as IUserCardView;

        public UserCardPresenter(ICardView view, CardModel model, EventBusManager eventBusManager) : base(view, model, eventBusManager)
        {
        }

        protected override void OnGameEventReceived(IGameEvent gameEvent)
        {
            base.OnGameEventReceived(gameEvent);
            switch (gameEvent)
            {
                case CardTargetsUpdateEvent targetsUpdateEvent:
                {
                    UpdateTargets(targetsUpdateEvent.TargetsDictionary);
                    break;
                }
            }
        }

        private void UpdateTargets(Dictionary<string, List<int>> targetsDictionary)
        {
            StopTargetEvents();
            Model.AvailableTargetIds = targetsDictionary;
        }

        public override void OnUserInput(UserInput input)
        {
            base.OnUserInput(input);

            if (input == UserInput.MouseDown)
            {
                ComputeDragBehaviour();
            }
        }

        public void ComputeDragBehaviour()
        {
            eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new StartDragEvent(Model));
            if (Model.ZoneId == Constants.HandZone)
            {
                //TODO: mandare un mesagio al player che gli dice che stai facendo drag
                //Model.Owner.ToggleDragInteraction(true, Model);
                HandleHandDrag();
            }
            else if (Model.ZoneId == Constants.BoardZone)
            {
                //TODO: mandare un mesagio al player che gli dice che stai facendo drag
                //Model.Owner.ToggleDragInteraction(true, Model);
                HandleBoardDrag();
            }
        }

        private void HandleBoardDrag()
        {
            //You can drag only ready cards
            if (!Model.IsReady || !Model.IsOwnerTurn)
            {
                return;
            }

            if (Model.CanFight && Model.AvailableTargetIds.TryGetValue(Constants.FightAction, out var fightTargets))
            {
                UserCard.PlaySelectAnimation(true);
                NotifyTargettableCards(fightTargets, true, false, true);

                UserCard.CreateTargetingArrow((id)=>
                {
                    StopTargetEvents();
                    OnFightTargetSelected(id);
                });
            }
            else if (Model.CanReap)
            {
                UserCard.PlaySelectAnimation(true);
                UserCard.CreateTargetingArrow((id)=>
                {
                    StopTargetEvents();
                    OnFightTargetSelected(id);
                });            }
        }

        private void NotifyTargettableCards(List<int> targets, bool started, bool replace, bool fight)
        {
            foreach (var target in targets)
            {
                eventBusManager.CardEventBus.Publish(target, new CardTargettableEvent(started, replace, fight));
            }
        }

        private void HandleHandDrag()
        {
            //You can drag only playable cards
            if (!Model.CanBePlayed || !Model.IsOwnerTurn)
            {
                return;
            }

            //If has a play ability...
            if (Model.AvailableTargetIds.TryGetValue(Constants.PlayAction, out var playTargets) && playTargets.Count > 0)
            {
                NotifyTargettableCards(playTargets, true, false, false);

                //Selects a target and play the card
                UserCard.CreateTargetingArrow((id)=>
                {
                    StopTargetEvents();
                    OnPlayTargetSelected(id);
                });
            }
            //If needs to replace a card...
            else if (Model.AvailableTargetIds.TryGetValue(Constants.ReplaceAction, out var replaceTargets) && replaceTargets.Count > 0)
            {
                NotifyTargettableCards(replaceTargets, true, true, false);

                //Selects a target and replace the card
                UserCard.CreateTargetingArrow((cardId) =>
                {
                    StopTargetEvents();
                    PlayCard(onReplaceCardID: cardId);
                });
            }
            else
            {
                UserCard.StartDrag(OnCardDragEnd);
            }
        }

        private void OnCardDragEnd()
        {
            StopTargetEvents();
            if (UserCard.RaycastHitOnGivenLayer(LayerMask.GetMask(Constants.GraveyardLayer)))
                eventBusManager.GameInteractor.DiscardCard(Model);

            else if (UserCard.RaycastHitOnGivenLayer(LayerMask.GetMask(Constants.BoardLayer)))
                eventBusManager.GameInteractor.PlayCard(Model);

            else
                UserCard.AnimateTo(ZoneTransform.position);

            //TODO: same stuff
            eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new EndDragEvent(Model));
            //Model.Owner.ToggleDragInteraction(false, Model);
        }

        private void StopTargetEvents()
        {
            if(Model.AvailableTargetIds.TryGetValue(Constants.FightAction, out var fightTargets))
            {
                foreach (var target in fightTargets)
                {
                    eventBusManager.CardEventBus.Publish(target, new CardTargettableEvent(false, false, false));
                }
            }
            if(Model.AvailableTargetIds.TryGetValue(Constants.PlayAction, out var playTargets))
            {
                foreach (var target in playTargets)
                {
                    eventBusManager.CardEventBus.Publish(target, new CardTargettableEvent(false, false, false));
                }
            }
            if(Model.AvailableTargetIds.TryGetValue(Constants.ReplaceAction, out var replaceTargets))
            {
                foreach (var target in replaceTargets)
                {
                    eventBusManager.CardEventBus.Publish(target, new CardTargettableEvent(false, false, false));
                }
            }
        }

        private void OnFightTargetSelected(int creatureId)
        {
            UserCard.PlaySelectAnimation(false);
            if (creatureId != -1 && creatureId != Model.InstanceId)
            {
                eventBusManager.GameInteractor.FightCreature(Model, creatureId);
            }
            else if (UserCard.RaycastHitOnGivenLayer(LayerMask.GetMask(Constants.ReapLayer)))
            {
                eventBusManager.GameInteractor.ReapCreature(Model);
            }
            else if (creatureId == -1 || creatureId == Model.InstanceId)
            {
                
            }

            //TODO: same stuff
            eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new EndDragEvent(Model));
            //Model.Owner.ToggleDragInteraction(false, Model);
        }

        private void OnPlayTargetSelected(int target)
        {
            //If the card must be replaced in order to be played...
            if (!UserCard.RaycastHitOnGivenLayer(LayerMask.GetMask(Constants.GraveyardLayer))
                && Model.AvailableTargetIds.TryGetValue(Constants.ReplaceAction, out var replaceTargets)
                && replaceTargets.Count > 0)
            {
                //Replace first and then select the play target...
                NotifyTargettableCards(replaceTargets, true, true, false);

                CoroutineHelper.Instance.StartCoroutineHelper(WaitAndSelectReplaceTarget(target));
            }
            else
            {
                PlayCard(onPlayCardId: target);
            }

            //TODO: same stuff
            eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new EndDragEvent(Model));
            //Model.Owner.ToggleDragInteraction(false, Model);
        }

        private IEnumerator WaitAndSelectReplaceTarget(int target)
        {
            yield return new WaitForEndOfFrame();

            UserCard.CreateTargetingArrow((int replaceTarget) =>
            {
                StopTargetEvents();
                PlayCard(target, replaceTarget);
            });
        }

        private void PlayCard(int onPlayCardId = -1, int onReplaceCardID = -1)
        {
            if (UserCard.RaycastHitOnGivenLayer(LayerMask.GetMask(Constants.GraveyardLayer)))
            {
                eventBusManager.GameInteractor.DiscardCard(Model);
                return;
            }

            eventBusManager.GameInteractor.PlayCard(Model, onPlay: onPlayCardId, replace: onReplaceCardID);
        }
    }
}
