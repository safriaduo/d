using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dawnshard.Views
{
    public class ShardStatView : StatView
    {
        [SerializeField] private MMFeedbacks gainAnimation;
        [SerializeField] private MMFeedbacks loseAnimation;
        [SerializeField] private MMFeedbacks winningAnimation;
        [SerializeField] private MMFeedbacks activePlayerStartShardIdleAnimation;        
        [SerializeField] private MMFeedbacks activePlayerShardIdleAnimation;
        [SerializeField] private MMFeedbacks endGameAnimation;
        [SerializeField] private GameObject miniShard;
        [SerializeField] private ShardStatView otherShard;
        [SerializeField] private MMFeedbackTMPText shardNumberText;
        [SerializeField] private Transform otherTransform;
        [SerializeField] public Transform shardTransform;
        [SerializeField] private Transform startShardMovement;
        [SerializeField] private Transform middleShardMovement;
        [SerializeField] private Transform endShardMovement;
        private int shardTotal = 0;
        private bool isPositionSet = false;
        private bool activePlayerAnimationPlaying = false;
        private bool hasShardAnimation;
        private int shardDifference;
        private int originalValue;
        private bool hasStats=false;
        private bool isSteal = false;

        private const float SHARD_MOVEMENT_ANIMATION_DURATION = .7f;

        /// <summary>
        /// Play the shard loss animation
        /// </summary>
        protected void LoseShardsAnimation(int animShardTotal)
        {
            loseAnimation.PlayFeedbacks();
            ToggleWinning(animShardTotal >= Constants.ShardToWin);
        }

        /// <summary>
        /// Play the shard gain animation
        /// </summary>
        private void GainShardsAnimation(int animShardTotal)
        {
            gainAnimation.PlayFeedbacks();
            ToggleWinning(animShardTotal >= Constants.ShardToWin);
        }

        /// <summary>
        /// Play the miniShard movement animation for each shard gained/lost
        /// </summary>
        /// <param name="miniShards">The list of miniShards"</param>
        /// <param name="target">Their destination</param>
        private void MoveMiniShardsAnimation(List<GameObject> miniShards, Transform target)
        {
            foreach (GameObject miniShard in miniShards)
            {
                middleShardMovement.position += startShardMovement.position;
                endShardMovement.position = target.position;
                miniShard.GetComponent<MMFeedbacks>().Initialization();
                miniShard.GetComponent<MMFeedbacks>().PlayFeedbacks();
            }
        }

        /// <summary>
        /// Toggle the winning animation if a player reach the winning number of shards or lose a shard and don't have that number of shards anymore
        /// </summary>
        /// <param name="enable"></param>
        private void ToggleWinning(bool enable)
        {
            if (enable) winningAnimation.PlayFeedbacks();
            else winningAnimation.StopFeedbacks();
        }

        /// <summary>
        /// Start the idle animation if the player that owns the shard this script refer to is the active player 
        /// </summary>
        /// <param name="active"></param>
        public void ChangeActivePlayer(bool active)
        {
            if (active && !activePlayerAnimationPlaying)
            {
                activePlayerStartShardIdleAnimation.PlayFeedbacks();
                activePlayerAnimationPlaying = true;
            }
            else if (!active && activePlayerAnimationPlaying)
            {
                activePlayerShardIdleAnimation.StopFeedbacks();
                activePlayerStartShardIdleAnimation.StopFeedbacks();
                activePlayerAnimationPlaying = false;
            }
        }


        private GameObject CreateMiniShard(Vector3 originPosition)
        {
            return Instantiate(miniShard,
                originPosition + new Vector3(
                    Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)
                    ), Quaternion.identity);
        }

        /// <summary>
        /// If the shard value is changed play all the animation related to that event
        /// </summary>
        public override void SetStat(int originalValue, int value, int maxValue=0)
        {
            if (shardTotal == value || originalValue == value) return;
            this.originalValue = originalValue;
            shardDifference = value - originalValue;
            shardTotal = value;
            base.SetStat(originalValue, value);
            hasStats=true;
            //if(isPositionSet)
                StartCoroutine(PlayShardAnimation(shardTotal,shardDifference, originalValue,hasShardAnimation,otherTransform));
        }

        private IEnumerator PlayShardAnimation(int animShardTotal, int animShardDifference, int animOriginalValue,
            bool animHasShardAnimation, Transform animOtherTransform)
        {
            isPositionSet = false;
            hasStats = false;
            shardNumberText.NewText = animShardTotal.ToString();
            if (animShardDifference < 0)
            {
                LoseShardsAnimation(animShardTotal);
            }
            if (animHasShardAnimation)
            {
                List<GameObject> miniShards = new List<GameObject>();
                for (int i = 0; i < MathF.Abs(animShardDifference); i++)
                {
                    startShardMovement.position = animShardDifference < 0 ? shardTransform.position : animOtherTransform.position;
                    miniShards.Add(CreateMiniShard(startShardMovement.position));
                }
                MoveMiniShardsAnimation(miniShards, animShardDifference < 0 ? animOtherTransform : shardTransform);
                yield return new WaitForSeconds(SHARD_MOVEMENT_ANIMATION_DURATION);
            }
            if (shardDifference > 0)
            {
                GainShardsAnimation(animShardTotal);
            }

            if (isSteal)
            {
                otherShard.SetShardAnimationInfo(Vector3.zero, false, false);
                isSteal = false;
            }
        }

        /// <summary>
        /// Set some info so that the animation can play properly
        /// </summary>
        /// <param name="card">Position of the card that has made the action that changs the number of shards</param>
        /// <param name="isSteal">Whether or not the action is steal</param>
        public void SetShardAnimationInfo(Vector3 card, bool isSteal, bool hasShardAnimation, bool isOpponentShard=false)
        {
            if (isOpponentShard)
            {
                otherShard.SetShardAnimationInfo(card, isSteal, hasShardAnimation);
                return;
            }
            if (isSteal)
            {
                this.isSteal = isSteal;
                otherTransform.position = otherShard.shardTransform.position;
                middleShardMovement.position = new Vector3(-10, 3f, 2f);
            }
            else
            {
                otherTransform.position = card;
                middleShardMovement.position = new Vector3(0, 3f, +4f);
            }
            isPositionSet = true;
            this.hasShardAnimation = hasShardAnimation;
            if (hasStats)
            {
                StartCoroutine(PlayShardAnimation(shardTotal,shardDifference, originalValue,hasShardAnimation,otherTransform));
            }
        }

        public IEnumerator WinAnimation()
        {
            endGameAnimation.PlayFeedbacks();
            yield return new WaitForSeconds(7f);
        }
    }
}