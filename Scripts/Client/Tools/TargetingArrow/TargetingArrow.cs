// Copyright (C) 2016-2021 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Spawn a Sprite that makes the line animation and shows the UI.
    /// Card selection is handled by the board creatures
    /// </summary>
    public class TargetingArrow : MonoBehaviour
    {
        public Transform arrowTipPosition;
        [SerializeField] private BoxCollider arrowCollider;
        [SerializeField] private Transform arrowTransform;

        public Action<int> OnTargetSelected { get; private set; }
        protected bool startedDrag;
        private Transform origin;

        private CardView lastCard;
        //public Ray ray;

        /// <summary>
        /// Determines the target card
        /// </summary>
        public int TargetId { get; protected set; }

        protected virtual void Update()
        {
            if (startedDrag)
            {
                if (origin == null || arrowCollider == null || arrowTipPosition == null)
                {
                    Destroy(gameObject);
                }
                if (Vector3.Distance(origin.position, arrowTipPosition.position) > arrowCollider.size.z*1.5f)
                    arrowTransform.localScale = Vector3.one;
                else
                    arrowTransform.localScale = Vector3.zero;
                
                transform.position = origin.position;

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                arrowTipPosition.position = ray.GetPoint(Camera.main.transform.position.y);
                
                Vector3 directionToCamera = (Camera.main.transform.position - arrowTipPosition.position).normalized;

                arrowTipPosition.position += 2f*directionToCamera;


                
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("CardInField")))
                {
                    if (hit.collider.TryGetComponent(out CardView cardView))
                    {
                        if (cardView != lastCard && lastCard != null)
                            lastCard.OnTargetingArrowRaycastHitNoMore();
                        TargetId = cardView.OnTargetingArrowRaycastHit();
                        lastCard = cardView;
                    }
                }
                else
                {
                    if (lastCard != null)
                    {
                        lastCard.OnTargetingArrowRaycastHitNoMore();
                        lastCard = null;
                        TargetId = -1;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    End();
                }
            }
        }

        /// <summary>
        /// Create the targeting arrow sprites that starts at position
        /// </summary>
        public virtual void Begin(Transform origin, Action<int> OnTargetSelected)
        {
            this.OnTargetSelected = OnTargetSelected;
            startedDrag = true;
            this.origin = origin;
            origin.position += 2f*Vector3.up; 
            arrowTransform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Destroys the targeting arrow gameobject
        /// </summary>
        public virtual void End()
        {
            if (!startedDrag)
            {
                return;
            }

            startedDrag = false;
            if(lastCard!=null)
                lastCard.OnTargetingArrowRaycastHitNoMore();
            OnTargetSelected?.Invoke(TargetId);
            Destroy(gameObject);
        }


        public virtual void OnCardSelected(int id)
        {
            TargetId = id;
        }

        /// <summary>
        /// The targeting arrow deselects the target
        /// </summary>
        public virtual void OnCardUnselected()
        {
            TargetId = -1;
        }
    }
}