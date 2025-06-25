using Dawnshard.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using safriaduo.UI;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Dawnshard.Presenters
{
    /// <summary>
    /// This class is the presenter of a card view. It's responsible to communicate game updates the card view 
    /// </summary>
    public class CardPresenter : BaseCardPresenter, ICardPresenter
    {
        protected bool isSelectingWorld = false;
        // protected string worldSelected = "";
        protected bool wasHighlighted = false;


        protected EventBusManager eventBusManager;

        public IGameCardView CardView => cardView as IGameCardView;

        public Transform ZoneTransform { get => CardView.ZoneTransform; set => CardView.ZoneTransform = value; }

        public CardPresenter(ICardView view, CardModel model, EventBusManager eventBusManager) : base(view, model)
        {
            if (eventBusManager != null)
            {
                this.eventBusManager = eventBusManager;
                eventBusManager.CardEventBus.Subscribe(Model.InstanceId, OnGameEventReceived);
                eventBusManager.PlayerEventBus.Subscribe(Model.OwnerId, OnOwnerEventReceived);
                view.SetOnDestroy(() =>
                {
                    eventBusManager.CardEventBus.Unsubscribe(Model.InstanceId, OnGameEventReceived);
                    eventBusManager.PlayerEventBus.Unsubscribe(Model.OwnerId, OnOwnerEventReceived);
                });
            }

            if (Model.ZoneId == Constants.BoardZone)
            {
                CardView.OnZoneChanged(Constants.HandZone, Constants.BoardZone);
            }
        }

        public override void Destroy()
        {
            eventBusManager.CardEventBus.Unsubscribe(Model.InstanceId, OnGameEventReceived);
            eventBusManager.PlayerEventBus.Unsubscribe(Model.OwnerId, OnOwnerEventReceived);

            base.Destroy();
        }

        private void OnOwnerEventReceived(IGameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case TurnStartedEvent:
                    {
                        Model.IsOwnerTurn = true;
                        isSelectingWorld = true;
                        SetDefaultHighlight();
                        break;
                    }

                case TurnStoppedEvent:
                    {
                        Model.IsOwnerTurn = false;
                        SetDefaultHighlight();
                        break;
                    }

                case ActiveWorldChangedEvent ev:
                    {
                        isSelectingWorld = ev.WorldSelected == false;
                        //worldSelected = ev.WorldId;
                        SetDefaultHighlight();
                        break;
                    }
                case HighlightCardEvent highlightCard:
                    {
                        SetDefaultHighlight();
                        break;
                    }

                default:
                    break;
            }
        }

        protected virtual void OnGameEventReceived(IGameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case CardAbilityTriggeredEvent abilityTriggered:
                    {
                        OnCardAbilityTriggered(abilityTriggered.Ability.TriggerId, abilityTriggered.Ability.EffectId);
                        break;
                    }

                case CardReadyChangedEvent readyChanged:
                    {
                        OnCardReadyChanged(readyChanged.CanFight, readyChanged.CanReap);
                        break;
                    }

                case CardWorldChangedEvent cardWorldChanged:
                    {
                        OnCardWorldChanged(cardWorldChanged.WorldId);
                        break;
                    }

                case CardStatChangedEvent statChanged:
                    {
                        OnCardStatChanged(statChanged.Stat, statChanged.PrevValue);
                        break;
                    }

                case PlayerAbilityTriggeredEvent playerAbility:
                    {
                        OnPlayerAbilityTriggered(playerAbility.TriggerId, playerAbility.EffectId, playerAbility.TargetPlayerIDs);
                        break;
                    }

                case CardKeywordChangedEvent keywordChanged:
                    {
                        OnCardKeywordChanged(keywordChanged.KeywordId, keywordChanged.Added);
                        break;
                    }

                case CardMovedEvent cardMoved:
                    {
                        // if (Model.Keywords.Contains(Constants.EphemeralKeyword) &&
                        //     cardMoved.DestZoneId == Constants.GraveyardZone)
                        // {
                        //     Destroy();
                        // }

                        OnCardMoved(cardMoved.OrigZoneId, cardMoved.DestZoneId);
                        SetDefaultHighlight();
                        break;
                    }

                case ReapCreatureTriggered reapCreatureEvent:
                    {
                        OnReap();
                        if (ZoneTransform != null)
                            eventBusManager.PlayerEventBus.Publish(Model.OwnerId,
                                new ChangeShardEvent(ZoneTransform.position, false, true));
                        else
                            eventBusManager.PlayerEventBus.Publish(Model.OwnerId,
                                new ChangeShardEvent(CardView.GetPosition(), false, true));
                        break;
                    }


                case FightCreatureTriggered fightCreatureTriggered:
                    {
                        eventBusManager.CardEventBus.Publish(
                            fightCreatureTriggered.AttackerId, new CardToAttackEvent(GetTargetWorldPosition()));
                        break;
                    }
                case CardToAttackEvent meetCardToAttack:
                    {
                        FightCreature(meetCardToAttack.DefenderCardPos - (meetCardToAttack.DefenderCardPos - GetTargetWorldPosition()).normalized * 3f + Vector3.up);
                        break;
                    }
                case CardTargettableEvent targettableEvent:
                    {
                        if (targettableEvent.StartedTarget)
                        {
                            wasHighlighted = true;
                            ToggleHighlight(true, Color.red);
                            CardView.ToggleTarget(targettableEvent.Replace, targettableEvent.Fight);

                        }
                        else
                        {
                            CardView.ToggleTarget(false, false);
                            SetDefaultHighlight();
                        }

                        break;
                    }
                case CardTargetsUpdateEvent targetsUpdate:
                    {
                        break;
                    }

                default:
                    {
                        Debug.LogWarning($"The game event of type {gameEvent.GetType()} is not handled by the card presenter.");
                        break;
                    }
            }
        }



        protected void SetTriggerView()
        {
            foreach (var ability in Model.Abilities)
            {
                switch (ability.TriggerId)
                {
                    case Constants.OnPlay:
                        continue;
                }

                CardView.SetTrigger(ability.TriggerId);
                return;
            }
        }

        #region ICardPresenter

        public virtual void UpdateView()
        {
            SetTriggerView();

            CardView.InstanceId = Model.InstanceId;

            // foreach (var stat in Model.Stats)
            // {
            //     CardView.ChangeStat(stat.ID, stat.OriginalValue, stat.Value, stat.Value, stat.MaxValue);
            // }

            UpdateKeywords();

            SetDefaultHighlight();
        }

        /// <summary>
        /// Show the keywords of the card, based on the zone
        /// </summary>
        private void UpdateKeywords()
        {
            if (Model.ZoneId != Constants.HandZone)
            {
                CardView.SetKeywords(Model.Keywords);
            }
            else
            {
                bool isEphemeral = Model.Keywords.Contains(Constants.EphemeralKeyword);
                List<string> handKws = new();

                if (isEphemeral)
                {
                    handKws.Add(Constants.EphemeralKeyword);
                }

                CardView.SetKeywords(handKws);
            }
        }

        public virtual void OnCardMoved(string origZone, string destZoneId)
        {
            // if (origZone == Constants.HandZone && !Model.IsOwnerLocalPlayer)
            //     FlipCard();
            Model.ZoneId = destZoneId;
            //Model.CanBePlayed = Model.CanBePlayed && destZoneId == Constants.HandZone;
            Action OnEndMovement = null;

            if (Model.Stats.Find(statModel => statModel.ID == Constants.ShardStat).Value > 0 &&
                origZone == Constants.HandZone && (destZoneId is Constants.BoardZone or Constants.ActionZone))
            {
                OnEndMovement = () => eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new ChangeShardEvent(ZoneTransform == null ? CardView.GetPosition() : ZoneTransform.position, false, true));
            }
            CardView.OnZoneChanged(origZone, destZoneId, OnEndMovement);
            if (destZoneId is not Constants.BoardZone)
            {
                cardView.SetWorld(Model.WorldId);
                foreach (var stat in Model.Stats)
                {
                    cardView.SetStat(stat.ID, stat.OriginalValue, stat.OriginalValue, stat.OriginalValue);
                }
            }
        }

        public virtual void OnReap()
        {
            CardView.PlayReapAnimation();
        }

        private void FightCreature(Vector3 targetPos)
        {
            CardView.PlayFightAnimation(targetPos);
        }

        public virtual void OnPlayerAbilityTriggered(string triggerID, string effectID,
            List<string> playerAbilityTargetPlayerIDs)
        {
            foreach (var playerIDs in playerAbilityTargetPlayerIDs)
            {

                if (effectID == Constants.StealShardEffect)
                {
                    eventBusManager.PlayerEventBus.Publish(playerIDs,
                        new ChangeShardEvent(ZoneTransform.position, true, true));
                }

                if (effectID == Constants.LoseShardEffect)
                {
                    eventBusManager.PlayerEventBus.Publish(playerIDs,
                        new ChangeShardEvent(ZoneTransform.position, false, false));
                }


                if (effectID == Constants.GainShardEffect)
                {
                    eventBusManager.PlayerEventBus.Publish(playerIDs,
                        new ChangeShardEvent(ZoneTransform.position, false, true));
                }
            }

            Debug.Log("Player ability card " + Model.InstanceId + " Trigger " + triggerID + " Effect " + effectID);

            // switch (effectID)
            // {
            //     case Constants.StealShardEffect:
            //         //TODO: implementare effetti
            //         break;
            //     default: 
            //         break;
            //
            // }
            if (Model.Type == Constants.ActionType)
                return;

            CardView.TriggerAbility(triggerID, effectID);
        }

        public virtual void OnCardAbilityTriggered(string triggerID, string effectID)
        {
            Debug.Log("Card ability card " + Model.InstanceId + " Trigger " + triggerID + " Effect " + effectID);

            if (effectID == Constants.LockShardEffect)
            {
                var shardPosition = ZoneTransform == null ? CardView.GetPosition() : ZoneTransform.position;
                eventBusManager.PlayerEventBus.Publish(Model.OwnerId, new ChangeShardEvent(shardPosition, false, true, true));
            }
            if (Model.Type == Constants.ActionType)
                return;

            CardView.TriggerAbility(triggerID, effectID);
        }

        public Vector3 GetTargetWorldPosition(string effectID = "")
        {
            return CardView.GetPosition();
        }

        public virtual void OnCardKeywordChanged(string keyword, bool added)
        {
            //TODO: When a card is destroyed by its own attack it raises an exception here
            Debug.Log("Keyword changed card " + Model.InstanceId + " Kw " + keyword + " added? " + added);

            if (added)
            {
                Model.Keywords.Add(keyword);
            }
            else
            {
                if (!Model.Keywords.Remove(keyword))
                {
                    Debug.LogWarning($"Removed a keyword that the card presenter is not aware of! Card: {Model.InstanceId}, Keyword: {keyword}");
                }
            }

            if (added)
            {
                CardView.AddKeyword(keyword);
            }
            else
            {
                CardView.RemoveKeyword(keyword);
            }

            //If it's neither exhaust nor static, we don't show the exhaust effect
            if (keyword != Constants.ExhaustKeyword || Model.Keywords.Contains(Constants.StaticKeyword))
                return;

            if (((!Model.IsOwnerTurn) || (Model.IsOwnerTurn && Model.Keywords.Contains(Constants.ExhaustKeyword))) && added)
            {
                CardView.ExhaustEffect(true);
            }
            else if (Model.IsOwnerTurn && !Model.Keywords.Contains(Constants.ExhaustKeyword))
            {
                CardView.ExhaustEffect(false);
            }
        }

        private void OnCardReadyChanged(bool canFight, bool canReap)
        {
            Debug.Log($"Card ready changed: fight: {Model.CanFight}, reap: {Model.CanReap}");

            Model.CanReap = canReap;
            Model.CanFight = canFight;

            SetDefaultHighlight();
            //ChangeCardFrame((Model.CanFight || Model.CanReap) || !isOwnerTurn);
        }

        public void ChangeCardFrame(bool growFrame)
        {
            if (Model.Type != Constants.CreatureType)
            {
                return;
            }
            if (growFrame)
            {
                CardView.GrowFrame();
            }
            else
            {
                CardView.ShrinkFrame();
            }
        }

        public virtual void OnCardStatChanged(StatModel stat, int prevValue)
        {
            Debug.Log("Stat changed card " + Model.InstanceId + " Stat " + stat.ID + " Prev value " + prevValue + "Curr value " + stat.Value);

            //Model update
            var statToUpdate = Model.Stats.Find(s => s.ID == stat.ID);
            if (statToUpdate != null)
            {
                statToUpdate.Value = stat.Value;
                statToUpdate.OriginalValue = stat.OriginalValue;
                statToUpdate.MaxValue = stat.MaxValue;
            }
            else
            {
                Model.Stats.Add(stat);
            }
            //Manage animation
            CardView.ChangeStat(stat.ID, stat.OriginalValue, stat.Value, prevValue, stat.MaxValue);
        }

        public virtual void OnCardWorldChanged(string worldId)
        {
            Model.WorldId = worldId;
            CardView.SetWorld(worldId);
        }

        public override void ToggleHighlight(bool enabled, Color color = default)
        {
            bool isInHand = Model.ZoneId == Constants.HandZone;
            cardView.ToggleHighlight(enabled, color, isInHand);
        }

        public virtual void AnimateTo(Vector3 position)
        {
            CardView.AnimateTo(position);
        }

        public void SetDefaultHighlight()
        {
            //if (!Model.IsOwnerTurn || Model.ZoneId == Constants.ArtifactZone || Model.ZoneId == Constants.ActionZone)
            if (!Model.CanBePlayed && !Model.IsReady || !Model.IsOwnerTurn)
            {
                ToggleHighlight(false);
            }
            else
            {
                //bool canBeUsed = (Model.CanReap || Model.CanFight || (Model.CanBePlayed&&Model.ZoneId==Constants.HandZone)) && (worldSelected==Model.WorldId || isSelectingWorld);

                Color color = isSelectingWorld ? Color.cyan : Color.white;

                ToggleHighlight(true, color);
            }
        }


        public void SetSortingGroupOrderInLayer(int i)
        {
            CardView.SetSortingGroupOrderInLayer(i);
        }

        public float GetMoveDuration(string origZone, string destZoneId)
        {
            return CardView.GetMoveDuration(origZone, destZoneId);
        }

        public float ReapDuration => CardView.ReapDuration;

        public float FightDuration => CardView.FightDuration;

        public float ReadyChangeDuration => CardView.ReadyChangeDuration;

        public float AbilityDuration => CardView.AbilityDuration;
        #endregion
    }
}