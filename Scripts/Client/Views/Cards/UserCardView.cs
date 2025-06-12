using Dawnshard.Network;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Types;
using MoreMountains.Feedbacks;

namespace Dawnshard.Views
{
    public class UserCardView : CardView, IUserCardView
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask playAreaLayer;
        [SerializeField] private LayerMask discardAreaLayer;
        [SerializeField] private LayerMask reapAreaLayer;

        [Header("Targeting")]
        [SerializeField] private CardTilter cardTilter;
        [SerializeField] private TargetingArrow targetingArrowPrefab;
        [SerializeField] private Color targetsColor = Color.red;


        private Vector3 defaultPosition;
        private float baseZCoord;
        private Vector3 baseOffset;
        
        protected Action DragEnded { get; set; }

        protected bool EnableDrag { get; set; }

        /// <summary>
        /// Spawn a targeting arrow
        /// </summary>
        public void CreateTargetingArrow(Action<int> onSelectTarget)
        {
            var spawnedArrow = Instantiate(targetingArrowPrefab);

            spawnedArrow.Begin(transform, onSelectTarget);
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = baseZCoord;
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

        private void Update()
        {
            if (EnableDrag)
            {
                transform.position = GetMouseWorldPos() + baseOffset;
            }
        }

        protected override void OnMouseDown()
        {
            if (UIManager.CurrentStateUI != UIManager.StateUI.None)
                return;
            OnUserInput?.Invoke(UserInput.MouseDown);
        }

        public override void OnMouseUp()
        {
            if (EnableDrag)
            {
                StopDrag();
            }

            base.OnMouseUp();
        }

        public void StartDrag(Action onCardDragEnd)
        {
            defaultPosition = transform.position;
            baseZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            baseOffset = defaultPosition - GetMouseWorldPos();
            cardTilter.enabled = true;
            DragEnded = onCardDragEnd;

            EnableDrag = true;
        }

        /// <summary>
        /// Stop the drag of the card
        /// </summary>
        protected void StopDrag()
        {
            cardTilter.enabled = false;
            EnableDrag = false;
            DragEnded?.Invoke();
        }

        public bool RaycastHitOnGivenLayer(LayerMask layer)
        {
            if (UIManager.CurrentStateUI != UIManager.StateUI.None)
                return false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out _, Mathf.Infinity, layer);
        }
    }
}
