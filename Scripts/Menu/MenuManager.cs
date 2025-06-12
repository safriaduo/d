using UnityEngine;
using System.Collections.Generic;
using Dawnshard.Menu;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Dawnshard.Menu
{
    /// <summary>
    /// The menu manager is a state machine, that will switch between state according to current gamestate.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private AState[] states;

        [Header("Transition")]
        [SerializeField] private MMFeedbacks transitionFeedbacks;
        [SerializeField] private MMFeedbacks homeFeedbacks;
        [SerializeField] private MMFeedbacks transitionToForgeFeedbacks;

        [Header("Options")]
        [SerializeField] private OptionButtonView optionButtonView;
        [SerializeField] private Image optionsBackground;

        [Header("Navigation")]
        [SerializeField] private UnityDictionary<Toggle> togglesByState;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button battlePassButton;

        static public MenuManager Instance { get { return s_Instance; } }
        static protected MenuManager s_Instance;

        public AState TopState { get { if (m_StateStack.Count == 0) return null; return m_StateStack[m_StateStack.Count - 1]; } }
        public AState HomeState => states[0];
        public AState BattlePassState => states[1];
        
        public AState forgeState = null;


        protected List<AState> m_StateStack = new List<AState>();
        protected Dictionary<string, AState> m_StateDict = new Dictionary<string, AState>();

        #region StatesHandling
        protected void Start()
        {
            s_Instance = this;

            // We build a dictionnary from state for easy switching using their name.
            m_StateDict.Clear();

            if (states.Length == 0)
                return;

            for (int i = 0; i < states.Length; ++i)
            {
                m_StateDict.Add(states[i].GetName(), states[i]);
            }

            m_StateStack.Clear();

            PushState(states[0].GetName());

            SetupNavigation();
        }

        private void SetupNavigation()
        {
            homeButton.onClick.AddListener(() =>
            {
                GoToState(HomeState.GetName());
                foreach (var toggle in togglesByState.keyValuePairs)
                {
                    toggle.item.isOn = false;
                }
            });
            battlePassButton.onClick.AddListener(() =>
            {
                GoToState(BattlePassState.GetName());
                foreach (var toggle in togglesByState.keyValuePairs)
                {
                    toggle.item.isOn = false;
                }
            });
            foreach (var toggle in togglesByState.keyValuePairs)
            {
                toggle.item.onValueChanged.AddListener((on) =>
                {
                    if (on)
                    {
                        GoToState(toggle.id);
                    }
                });
            }
        }

        /// <summary>
        /// Change the top state
        /// </summary>
        private IEnumerator SwitchState(string newState)
        {
            AState state = FindState(newState);
            if (state == null)
            {
                Debug.LogError("Can't find the state named " + newState);
                yield break;
            }

            m_StateStack[m_StateStack.Count - 1].Exit(state);
            yield return new WaitUntil(()=>!m_StateStack[m_StateStack.Count - 1].gameObject.activeSelf);
            state.Enter(m_StateStack[m_StateStack.Count - 1]);
            m_StateStack.RemoveAt(m_StateStack.Count - 1);
            m_StateStack.Add(state);
        }

        /// <summary>
        /// Return state for name
        /// </summary>
        private AState FindState(string stateName)
        {
            AState state;
            if (!m_StateDict.TryGetValue(stateName, out state))
            {
                return null;
            }

            return state;
        }

        /// <summary>
        /// Close the current top state
        /// </summary>
        private void PopState()
        {
            if (m_StateStack.Count < 2)
            {
                Debug.LogError("Can't pop states, only one in stack.");
                return;
            }

            m_StateStack[m_StateStack.Count - 1].Exit(m_StateStack[m_StateStack.Count - 2]);
            m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 2]);
            m_StateStack.RemoveAt(m_StateStack.Count - 1);
        }

        /// <summary>
        /// Push a new state to the top
        /// </summary>
        private void PushState(string name)
        {
            AState state;
            if (!m_StateDict.TryGetValue(name, out state))
            {
                Debug.LogError("Can't find the state named " + name);
                return;
            }

            if (m_StateStack.Count > 0)
            {
                m_StateStack[m_StateStack.Count - 1].Exit(state);
                state.Enter(m_StateStack[m_StateStack.Count - 1]);
            }
            else
            {
                state.Enter(null);
            }
            m_StateStack.Add(state);
        }
        #endregion


        #region TransitionHandling
        /// <summary>
        /// Options background sprite
        /// </summary>
        /// <param name="sprite"></param>
        public void SetOptionsBackground(Sprite sprite)
        {
            optionsBackground.sprite = sprite;
        }
        
        public void SetOptionsPosition(Transform transform)
        {
            optionsBackground.gameObject.transform.position = transform.position;
        }

        private IEnumerator OnTransitionCompleted(string state)
        {
            yield return SwitchState(state);

            yield return new WaitForSeconds(.2f);

            if (TopState == HomeState || TopState == BattlePassState)
                PlayHomeFeedback(); //Open the menu after being closed
            else if (TopState == forgeState)
                PlayForgeTransition();
            else
                PlayTransition(); //Open the menu after being closed
        }

        public void GoToState(string stateName, bool forceToggleSwitch = false)
        {
            if (TopState.GetName() == stateName) return;

            void onComplete() => StartCoroutine(OnTransitionCompleted(stateName));

            if (TopState == HomeState || TopState == BattlePassState)
            {
                PlayHomeFeedback(onComplete);
            }
            else if (TopState == forgeState)
            {
                PlayForgeTransition(onComplete);
            }
            else
            {
                PlayTransition(onComplete);
            }

            if (forceToggleSwitch)
            {
                if(stateName == Constants.HomeState)
                {
                    foreach (var toggle in togglesByState.keyValuePairs)
                    {
                        toggle.item.isOn = false;
                    }
                    return;
                }
                togglesByState.GetItem(stateName).isOn = true;
            }
        }

        /// <summary>
        /// Return to home feedback
        /// </summary>
        private void PlayHomeFeedback(Action onComplete = null)
        {
            homeFeedbacks.Events.OnComplete.RemoveAllListeners();

            if (onComplete != null)
            {
                homeFeedbacks.Events.OnComplete.AddListener(() => onComplete.Invoke());
            }

            homeFeedbacks.PlayFeedbacks();
        }
        
        /// <summary>
        /// Return to home feedback
        /// </summary>
        private void PlayForgeTransition(Action onComplete = null)
        {
            transitionToForgeFeedbacks.Events.OnComplete.RemoveAllListeners();

            if (onComplete != null)
            {
                transitionToForgeFeedbacks.Events.OnComplete.AddListener(() => onComplete.Invoke());
            }

            transitionToForgeFeedbacks.PlayFeedbacks();
        }

        /// <summary>
        /// Play animation of transition
        /// </summary>
        private void PlayTransition(Action onComplete = null)
        {
            transitionFeedbacks.Events.OnComplete.RemoveAllListeners();

            if (onComplete != null)
            {
                transitionFeedbacks.Events.OnComplete.AddListener(() => onComplete.Invoke());
            }

            transitionFeedbacks.PlayFeedbacks();

        }

        /// <summary>
        /// Set the submenu options buttons
        /// </summary>
        public void SetOptions(Dictionary<string, Action> options, string invokeOption, Dictionary<string, bool> notifications = null,
            Dictionary<string, bool> lockedOptions=null)
        {
            optionButtonView.SetOptions(options, invokeOption, notifications, lockedOptions);
        }
        
        #endregion
    }

    public abstract class AState : MonoBehaviour
    {
        [Header("Base Refs")]
        [SerializeField] protected Sprite optionBackground;
        
        public bool isLikeHome = false;
        protected Dictionary<string, bool> notifications = new Dictionary<string, bool>();
        protected Dictionary<string, bool> lockedOptions = new Dictionary<string, bool>();

        public virtual void Enter(AState from)
        {
            gameObject.SetActive(true);

            if (optionBackground != null)
            {
                MenuManager.Instance.SetOptionsBackground(optionBackground);
            }
        }

        public virtual void Exit(AState from)
        {
            gameObject.SetActive(false);
        }

        public void SetNotifications(Dictionary<string,bool> notifications)
        {
            this.notifications = notifications;
        }
        
        public void SetLockedOptions(Dictionary<string,bool> lockedOptions)
        {
            this.lockedOptions = lockedOptions;
        }
        
        public abstract string GetName();
    }
}