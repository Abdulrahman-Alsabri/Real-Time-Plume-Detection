using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // a reference to player object
    public Transform player;

    // declare and initialize needed variables
    public float zoom = 0f;
    public float zoomSpeed = 5.0f;
    public float zoomLowerLimit = -3f;
    public float zoomUpperLimit = -17f;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 3f, -17f);

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
        // update the desired position of the camera
        Vector3 desiredPosition = player.position + offset;

        // smoothens the movement to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // change camera's position
        transform.position = smoothedPosition;

        // always keep the camera looking at player
        transform.LookAt(player);

        // handle zooming
        zoom = Input.GetAxis("Mouse ScrollWheel");
        if (((offset.z + (zoom * zoomSpeed)) < zoomLowerLimit) &&
            ((offset.z + (zoom * zoomSpeed)) >= zoomUpperLimit))
        {
            offset.z += zoom * zoomSpeed;
        }

    }
}
