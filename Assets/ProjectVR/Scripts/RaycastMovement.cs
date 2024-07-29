using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RaycastMovement : MonoBehaviour {
    // References to game objects in the scene
    public GameObject raycastHolder;         // Holds the raycast position and direction
    public GameObject player;               // The player object
    public GameObject raycastIndicator;     // Indicator for movement

    // Adjustable parameters
    public float height = 2;                // Height offset for movement
    public bool teleport = true;            // Determines if teleportation is enabled
    public float maxMoveDistance = 10;      // Maximum distance for movement

    private bool moving = false;           // Flag to track if the player is currently moving

    RaycastHit hit;                        // Raycast hit information
    float theDistance;                     // Distance from raycast to hit point

    // Start is called before the first frame update
    void Start () {
        // Initialization code (if any) goes here
    }
    
    // Update is called once per frame
    void Update () {
        // Cast a ray in the forward direction
        Vector3 forwardDir = raycastHolder.transform.TransformDirection (Vector3.forward) * 100;
        Debug.DrawRay (raycastHolder.transform.position, forwardDir, Color.green);

        if (Physics.Raycast (raycastHolder.transform.position, (forwardDir), out hit)) {
            // If the ray hits an object
            Debug.Log(" Raycast Hit");

            if (hit.collider.gameObject.tag == "movementCapable") {
                // If the hit object has the tag "movementCapable"
                Debug.Log(" Raycast Hit movementCapable object");

                ManageIndicator ();  // Call the function to manage the indicator

                if (hit.distance <= maxMoveDistance) { 
                    // If the hit is within the maximum movement distance
                    Debug.Log(" Raycast Hit movementCapable object close enough");

                    if (raycastIndicator.activeSelf == false) {
                        raycastIndicator.SetActive (true); // Activate the indicator if not already active
                    }

                    if (Input.GetMouseButtonDown (0)) {
                        if (teleport) {
                            teleportMove (hit.point);   // Teleport the player to the hit point
                        } else {
                            DashMove (hit.point);       // Move the player to the hit point with a dash
                        }
                    }
                } else {
                    Debug.Log(" Raycast Hit movementCapable object away");
                    if (raycastIndicator.activeSelf == true) {
                        raycastIndicator.SetActive (false); // Deactivate the indicator if not within range
                    }
                }
            }
            else if (hit.collider.gameObject.tag == "clickable") {
                // If the hit object has the tag "clickable"
                Debug.Log(" Raycast Hit clickable object");

                // Trigger pointer enter event
                EventTrigger trigger = hit.collider.gameObject.GetComponent<EventTrigger>();
                if (trigger == null) {
                    trigger = hit.collider.gameObject.transform.parent.gameObject.GetComponent<EventTrigger>();
                }
                trigger.OnPointerEnter(null);

                if (Input.GetMouseButton (0)) {
                    trigger.OnPointerClick(null); // Trigger click event if mouse button is pressed
                }
            }
            else {
                Debug.Log(" Raycast Hit object tag " + hit.collider.gameObject.tag + " name " + hit.collider.gameObject.name);
            }
        }
    }

    // Function to manage the movement indicator
    public void ManageIndicator() {
        if (!teleport) {
            if (moving != true) {
                raycastIndicator.transform.position = hit.point; // Update indicator position
            }

            if(Vector3.Distance(raycastIndicator.transform.position, player.transform.position) <= 2.5) {
                moving = false; // Stop moving if close enough to the indicator
            }
        } else {
            raycastIndicator.transform.position = hit.point; // Update indicator position
        }
    }

    // Function for dashing movement
    public void DashMove(Vector3 location) {
        moving = true;

        // Use iTween to smoothly move the player to the new position
        iTween.MoveTo (player, 
            iTween.Hash (
                "position", new Vector3 (location.x, location.y + height, location.z), 
                "time", .2F, 
                "easetype", "linear"
            )
        );
    }

    // Function for teleportation movement
    public void teleportMove(Vector3 location) {
        player.transform.position = new Vector3 (location.x, location.y + height, location.z); // Set player position directly
    }
}
