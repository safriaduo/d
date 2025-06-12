using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // Adjust to taste

    void Update()
    {
        // Ray from the camera through the mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a horizontal plane at the deck's current Y position
        Plane plane = new Plane(Vector3.forward, -new Vector3(transform.position.x, transform.position.y, -100f));

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            // Point where the mouse ray hits the plane
            Vector3 hitPoint = ray.GetPoint(distance);

            // Direction from deck to that point
            Vector3 direction = (hitPoint - transform.position).normalized;
            
            direction = new Vector3(-direction.x, -direction.y, direction.z);

            // Target rotation so that the deck's forward (Z-axis) faces the direction
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Smoothly rotate from current rotation to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}