using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// The CharacterManager class manages the placement and removal of objects in an AR scene.
// It allows players to place an object by touching a detected plane and remove an object by touching it again.
public class PickupRaycast : MonoBehaviour
{
    // Reference to the ARRaycastManager component, which handles raycasting onto AR planes.
    public ARRaycastManager raycastManager;

    // Update is called once per frame.
    // Here we detect touch inputs and handle object placement and removal based on touch interactions.
    void Update()
    {
        // Check if there's at least one touch input on the screen.
        if (Input.touchCount > 0)
        {
            // Get the first touch event (useful if we're only using single-touch interactions).
            Touch touch = Input.GetTouch(0);
            // Check if the touch is on a UI element
            /*if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return; // Ignore the touch if it's on a UI element
            }*/
            // Check if the touch phase just began (indicating the player has just touched the screen).
            if (touch.phase == TouchPhase.Began)
            {
                // Create a ray from the camera through the touch position on the screen.
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Perform a raycast to check if the touch intersects with any objects in the AR scene.
                if (Physics.Raycast(ray, out hit))
                {
                    // Get the GameObject that was hit by the raycast.
                    GameObject touchedObject = hit.collider.gameObject;

                    
                }

            }
        }
    }
}





