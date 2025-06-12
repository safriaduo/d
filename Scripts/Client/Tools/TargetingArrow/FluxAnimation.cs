using UnityEngine;

namespace Dawnshard.Views
{
    public class FluxAnimation : MonoBehaviour
    {
        public Transform destination;
        public Vector3 defaultScale = new(0.0345f, 0.0345f, 0.0345f);

        private void Update()
        {
            UpdatePosition(destination.transform.position);
        }

        public void UpdatePosition(Vector3 targetPosition)
        {
            PointTo(targetPosition);
            SetScale(targetPosition);
        }

        public void PointTo(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            Quaternion rotation = Quaternion.FromToRotation(transform.up, direction);
            transform.rotation = rotation * transform.rotation;
        }

        public void SetScale(Vector3 targetPosition)
        {
            // Calculate the distance between the starting and target positions
            float distance = Vector3.Distance(transform.position, targetPosition);

            // Set the sprite's scale based on the distance
            Vector3 scale = defaultScale * distance;
            transform.localScale = scale;
        }
    }
}
