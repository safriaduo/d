using Dawnshard.Presenters;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


namespace Dawnshard.Views
{
    /// <summary>
    /// This component is the view of the card inside a match, it handles the dynamic data of the cards and the animations
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CardView : BaseCardView, IGameCardView
    {
        //Used to set the zone transform to feedback animations
        protected const string zoneTransformKey = "GoToZoneTransform";
        protected const string fightCreatureKey = "Fight";

        [Header("Dynamic UI")]
        [SerializeField]
        protected SpriteRenderer triggerIcon;

        [SerializeField] protected UnityDictionary<GameObject> keywordsById;
        [SerializeField] protected UnityDictionary<Sprite> iconByTriggerIds;
        [SerializeField] protected Collider interactionCollider;

        [Header("Animations")]
        [SerializeField] protected MMFeedbacks growFrameAnimation;
        [SerializeField] protected MMFeedbacks shrinkFrameAnimation;
        [SerializeField] protected MMFeedbacks reapAnimation;
        [SerializeField] protected MMFeedbacks fightAnimation;
        [SerializeField] protected MMFeedbacks triggerActivatedAnimation;
        [SerializeField] protected MMFeedbacks replaceSelectionAnimation;
        [SerializeField] protected MMFeedbacks replaceDeselectionAnimation;
        [SerializeField] protected MMFeedbacks damageAnimation;
        [SerializeField] protected MMFeedbacks damageNoWaveAnimation;
        [SerializeField] protected MMFeedbacks healAnimation;
        [SerializeField] protected MMFeedbacks buffAnimation;
        [SerializeField] protected MMFeedbacks debuffAnimation;

        [SerializeField] protected TMP_Text healthText;
        [SerializeField] protected TMP_Text attackText;


        [SerializeField] protected UnityDictionary<UnityDictionary<MMFeedbacks>> animationByMovementIds;
        [SerializeField] protected UnityDictionary<SettingByZone> settingByZoneIds;

        [SerializeField] protected SortingGroup cardSortingGroup;

        [SerializeField] protected List<Transform> transformToResetAfterMovement;
        protected List<Vector3> originalPositionToResetAfterMovement = new List<Vector3>();


        //protected Tween<Vector3> previousTween;

        /// <summary>
        /// When this is true, the card doesn't perform external animations
        /// </summary>
        protected bool ignoreAnimations = false;

        /// <summary>
        /// This is the object you have to activate for highlighting the card
        /// </summary>
        protected float selectHighlgihtYPosition = 0f;
        protected float baseYPosition = 0f;
        protected bool isReplacable = false;
        protected bool isFightable = false;
        protected bool selectedAnimation = false;
        protected bool isFrameBig = true;
        protected bool hasAttacked = false;
        protected bool isArtifact = false;

        private void Awake()
        {
            SetupIgnoreAnimations();
            ZoneTransform = transform;
        }

        private void Start()
        {
            originalPositionToResetAfterMovement.AddRange(transformToResetAfterMovement.ToList().ConvertAll(transformToReset=>transformToReset.localPosition));
            growFrameAnimation.Events.OnComplete.AddListener(ResetLocalPositions);
            shrinkFrameAnimation.Events.OnPlay.AddListener(ResetLocalPositions);
        }

        public override void SetCardType(string cardType, string incandescense)
        {
            base.SetCardType(cardType, incandescense);
            isArtifact = cardType == Constants.ArtifactType;
        }

        private void SetupIgnoreAnimations()
        {
            foreach (var kvp in animationByMovementIds.keyValuePairs)
            {
                foreach (var animation in kvp.item.keyValuePairs)
                {
                    animation.item.Events.OnPlay.AddListener(() => ignoreAnimations = true);
                    animation.item.Events.OnResume.AddListener(() => ignoreAnimations = true);
                    animation.item.Events.OnComplete.AddListener(() => ignoreAnimations = false);
                    animation.item.Events.OnPause.AddListener(() => ignoreAnimations = false);
                }
            }
        }

        #region ICardView

        public int InstanceId { get; set; }

        public Transform ZoneTransform { get; set; }

        public Action<UserInput> OnUserInput { get; set; }

        public virtual void SetKeywords(List<string> ids)
        {
            foreach (var keyword in keywordsById.keyValuePairs)
            {
                ToggleKeywordObjects(keyword.id, ids.Contains(keyword.id));
            }
        }

        public virtual void AddKeyword(string id)
        {
            ToggleKeywordObjects(id, true);
        }

        public virtual void RemoveKeyword(string id)
        {
            ToggleKeywordObjects(id, false);
        }

        public virtual void ChangeStat(string id, int originalValue, int value, int prevValue, int statMaxValue)
        {
            switch (id)
            {
                case Constants.HealthStat when prevValue > value:
                    //damage
                    healthText.text = $"-{prevValue - value}";
                    if (hasAttacked)
                    {
                        damageNoWaveAnimation.PlayFeedbacks();
                        hasAttacked = false;
                    }
                    else
                    {
                        damageAnimation.PlayFeedbacks();
                    }
                    break;
                case Constants.HealthStat when prevValue < value:
                    //heal
                    healthText.text = $"+{value - prevValue}";
                    healAnimation.PlayFeedbacks();
                    break;
                case Constants.AttackStat when prevValue > value:
                    //debuff
                    attackText.text = $"-{prevValue - value}";
                    debuffAnimation.PlayFeedbacks();
                    break;
                case Constants.AttackStat when prevValue < value:
                    //buff
                    attackText.text = $"+{value - prevValue}";
                    buffAnimation.PlayFeedbacks();
                    break;
            }

            UpdateStat(id, originalValue, value, statMaxValue);
        }


        public virtual void GrowFrame()
        {
            if (isFrameBig || isArtifact)
                return;
            StartCoroutine(WaitForNoMovementAndEnd(growFrameAnimation.PlayFeedbacks));
            isFrameBig = true;
        }

        public virtual void ShrinkFrame()
        {
            if (!isFrameBig || isArtifact)
                return;
            StartCoroutine(WaitForNoMovementAndEnd(shrinkFrameAnimation.PlayFeedbacks));
            isFrameBig = false;
        }

        public virtual void SetTrigger(string triggerId)
        {
            if (string.IsNullOrEmpty(triggerId) || triggerId == Constants.OnPlay)
            {
                //triggerIcon.gameObject.SetActive(false);
                return;
            }

            var spriteObj = iconByTriggerIds.GetItem(triggerId);

            if (spriteObj == null)
            {
                Debug.LogWarning($"Cannot find icon for trigger id {triggerId}");
                return;
            }

            //triggerIcon.gameObject.SetActive(true);
            if (triggerIcon.gameObject != null)
                triggerIcon.sprite = spriteObj;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public virtual void TriggerAbility(string triggerID, string effectID)
        {
            triggerActivatedAnimation.PlayFeedbacks();
        }

        public virtual void OnZoneChanged(string origZone, string destZone, Action OnEndAction = null)
        {
            var origZoneAnimations = animationByMovementIds.GetAllItems(origZone);
            SetKeywordList(false, null, null);
            foreach (var dict in origZoneAnimations)
            {
                var moveFeedbacks = dict.GetAllItems(destZone);

                foreach (var feedbacks in moveFeedbacks)
                {
                    if (feedbacks != null)
                    {
                        void OnEnd()
                        {
                            OnEndAction?.Invoke();
                            SetZoneSettings(destZone);
                            feedbacks.Events.OnComplete.RemoveListener(OnEnd);
                        }

                        SetDynamicTransforms(feedbacks);
                        feedbacks.FeedbacksIntensity = isArtifact && (destZone == Constants.BoardZone || destZone == Constants.HandZone && origZone == Constants.BoardZone)? 0 : 1;
                        feedbacks.PlayFeedbacks();
                        feedbacks.Events.OnComplete.AddListener(OnEnd);
                    }
                }
            }

        }

        public virtual void PlayReapAnimation()
        {
            reapAnimation.PlayFeedbacks();
        }

        public virtual void PlayFightAnimation(Vector3 destination)
        {
            SetDynamicTransforms(fightAnimation);

            var fightFeedback = fightAnimation.Feedbacks.Find(f => f.Label == fightCreatureKey) as MMFeedbackPosition;

            if (fightFeedback != null)
                fightFeedback.DestinationPosition = destination; //todo mettere nemico

            fightAnimation.PlayFeedbacks();

            hasAttacked = true;
        }

        public void ToggleTarget(bool replace, bool fight)
        {
            isReplacable = replace;
            isFightable = fight;
        }

        public void SetSortingGroupOrderInLayer(int i)
        {
            cardSortingGroup.sortingOrder = i * 100;
        }

        public void ExhaustEffect(bool enabled)
        {
            var objs = keywordsById.GetAllItems(Constants.ExhaustKeyword);

            foreach (var obj in objs)
            {
                if (obj != null)
                    obj.SetActive(enabled);
            }
        }

        public virtual void AnimateTo(Vector3 position)
        {
            if (ignoreAnimations) return;

            //previousTween?.Stop();

            transform.DOMove(position, .25f).SetEase(Ease.OutExpo);
        }

        private void ResetLocalPositions()
        {
            for (int i = 0;i<transformToResetAfterMovement.Count;i++)
            {
                transformToResetAfterMovement[i].localPosition=originalPositionToResetAfterMovement[i];
            }
        }

        public void PlaySelectAnimation(bool selected)
        {
            if (!selectedAnimation && selected)
            {
                if (selectHighlgihtYPosition == 0)
                {
                    baseYPosition = transform.position.y;
                    selectHighlgihtYPosition = baseYPosition + 2;
                }
                transform.position = new Vector3(transform.position.x, selectHighlgihtYPosition, transform.position.z);
                selectedAnimation = true;
            }
            else if (selectedAnimation && !selected)
            {
                transform.position = new Vector3(transform.position.x, baseYPosition, transform.position.z);
                selectedAnimation = false;
            }
        }

        #endregion

        #region Interactions

        public void OnMouseEnter()
        {
            OnUserInput?.Invoke(UserInput.HoverStarted);
        }

        public void OnMouseExit()
        {
            OnUserInput?.Invoke(UserInput.HoverEnded);
        }

        public void OnTargetingArrowRaycastHitNoMore()
        {
            if (isReplacable)
                replaceDeselectionAnimation.PlayFeedbacks();
            if (isFightable)
            {
                PlaySelectAnimation(false);
            }
        }

        public int OnTargetingArrowRaycastHit()
        {
            if (isReplacable)
                replaceSelectionAnimation.PlayFeedbacks();
            if (isFightable)
            {
                PlaySelectAnimation(true);
            }

            return InstanceId;
        }

        #endregion


        protected IEnumerator WaitAndEnd(Action onEnd)
        {
            yield return new WaitForSeconds(2f);
            onEnd.Invoke();
        }

        protected IEnumerator WaitForNoMovementAndEnd(Action onEnd)
        {
            Vector3 previousPosition = transform.position;
            yield return new WaitForSeconds(0.2f);
            while (previousPosition != transform.position)
            {
                previousPosition = transform.position;
                yield return new WaitForSeconds(0.2f);
            }

            onEnd.Invoke();
        }

        /// <summary>
        /// Toggle the objects which cannot be shown.
        /// This toggles the container, so it doesn't interfere with keywords toggling
        /// </summary>
        protected void SetZoneSettings(string zoneId)
        {
            foreach (var objectByZone in settingByZoneIds.keyValuePairs)
            {
                //We need to always activate or deactivate the objects
                foreach (var obj in objectByZone.item.objToActivate)
                {
                    obj.SetActive(objectByZone.id == zoneId);
                }

                if (objectByZone.id == zoneId)
                {
                    foreach (var group in objectByZone.item.sortingGroups)
                    {
                        group.sortingLayerName = objectByZone.item.sortingLayerName;
                    }
                }
            }

            if (zoneId == Constants.BoardZone)
            {
                gameObject.layer = LayerMask.NameToLayer("CardInField");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Card");
            }
        }

        /// <summary>
        /// Toggles a keyword effect
        /// </summary>
        protected void ToggleKeywordObjects(string id, bool enabled)
        {
            var objs = keywordsById.GetAllItems(id);

            foreach (var obj in objs)
            {
                if (obj != null)
                    obj.SetActive(enabled);
            }
        }

        /// <summary>
        /// Set the transforms for the feedback based on the current zone transform
        /// </summary>
        protected void SetDynamicTransforms(MMFeedbacks feedbacks)
        {
            foreach (var subFeedback in feedbacks.Feedbacks)
            {
                if (subFeedback != null && subFeedback.Label == zoneTransformKey)
                {
                    switch (subFeedback)
                    {
                        case MMFeedbackPosition f:
                            {
                                f.DestinationPositionTransform = ZoneTransform;
                                break;
                            }

                        case MMFeedbackDestinationTransform f:
                            {
                                f.Destination = ZoneTransform;
                                break;
                            }

                        default:
                            {
                                Debug.LogWarning(
                                    $"A subfeedback of {feedbacks.name} has name matching zone transform key: {zoneTransformKey} but it's not supported. Please change it.");
                                break;
                            }
                    }
                }
            }
        }

        public virtual void OnMouseUp()
        {
            OnUserInput?.Invoke(UserInput.MouseUp);
        }


        [Serializable]
        public class SettingByZone
        {
            [Tooltip("These objects are activated only when the card is in the zone, they are disabled if they aren't")]
            public List<GameObject> objToActivate = new();

            public List<SortingGroup> sortingGroups = new();

            public string sortingLayerName;
        }
    }
}