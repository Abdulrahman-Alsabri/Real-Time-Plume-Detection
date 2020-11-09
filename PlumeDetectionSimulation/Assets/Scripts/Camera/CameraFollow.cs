using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // a reference to player object
    public Transform player;

    // declare and initialize needed variables
    public float zoom = 0f;
    public float zoomSpeed = 10.0f;
    public float zoomLowerLimit = -3f;
    public float zoomUpperLimit = -17f;
    public float smoothSpeed = 2f;
    public Vector3 offset = new Vector3(0f, 3f, -17f);
    public Vector3 desiredCamPosition = new Vector3(150f, 300f, 150f);
    public Vector3 desiredAngles = new Vector3(90f, 0f, 0f);
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;

    public bool isEnabled = true;
    public bool isSetPath = false;

    // Start is called before the first frame update
    void Start()
    {
        // searches for player object
        GameObject temp = GameObject.Find("_Boat");
        if (temp != null)
        {
            // gets player's transform info
            player = temp.GetComponent<Transform>();
        }
        else
        {
            Debug.Log("_Boat not found");
        }
    }

    /* Fixed Update can run once, zero, or several times per frame,
     * depending on how many physics frames per second are set in the
     * time settings, and how fast/slow the framerate is
     */
    void FixedUpdate()
    {
        if (isEnabled)
        {
            if (isSetPath)
            {
                // update the desired position of the camera
                desiredPosition = desiredCamPosition;

                // always keep the camera looking at player
                transform.eulerAngles = desiredAngles;

                if (transform.position.y >= 295)
                {
                    isEnabled = false;
                }
            }
            else
            {
                // update the desired position of the camera
                desiredPosition = player.position + offset;

                // always keep the camera looking at player
                transform.LookAt(player);
            }

            // smoothens the movement to the desired position
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // change camera's position
            transform.position = smoothedPosition;
        }
    }

    private void setCameraToSetPath()
    {
        
    }

    private void setCameraToFollowBoat()
    {
        
    }
}
