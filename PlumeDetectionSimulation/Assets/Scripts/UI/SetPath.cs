using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetPath : MonoBehaviour
{

    public CameraFollow camFollowScript;
    public BoatAttack.Boat boat;
    public Plume plumeScript;
    public GameObject pathPointMarker;
    public GameObject readingMarker;

    public GameObject respawnPlume;
    public GameObject showHidePlume;
    public GameObject restart;
    public GameObject exit;
    public GameObject miniMapFrame;
    public GameObject infoFrame;

    public Queue<Vector3> pathPoints = new Queue<Vector3>();
    public Queue<GameObject> pathPointMarkers = new Queue<GameObject>();
    public Queue<GameObject> pathPointTempMarkers = new Queue<GameObject>();

    private int buttonCounter = 0;
    private bool triggerSettingPathPoints = false;


    void Start()
    {
        GameObject temp = GameObject.Find("Main Camera");
        if (temp != null)
        {
            camFollowScript = temp.GetComponent<CameraFollow>();
        }
        else
        {
            Debug.Log("Main Camera not found");
        }

        GameObject temp2 = GameObject.Find("_Boat");
        if (temp2 != null)
        {
            boat = temp2.GetComponent<BoatAttack.Boat>();
        }
        else
        {
            Debug.Log("_Boat not found");
        }

        GameObject temp3 = GameObject.Find("SeaCollider");
        if (temp3 != null)
        {
            plumeScript = temp3.GetComponent<Plume>();
        }
        else
        {
            Debug.Log("SeaCollider not found");
        }

        // Search for PathPointMarker
        pathPointMarker = Resources.Load("PathPointMarker") as GameObject;
        readingMarker = Resources.Load("ReadingMarker") as GameObject;
    }

    void Update()
    {
        if (triggerSettingPathPoints && !boat.getNextPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPosition = -Vector3.one;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    clickPosition = hit.point;
                }

                if (clickPosition != -Vector3.one)
                {
                    pathPoints.Enqueue(new Vector3(hit.point.x, 0f, hit.point.z));

                    // apply PathPointMarker
                    GameObject marker = Instantiate(pathPointMarker, hit.point, Quaternion.identity);
                    pathPointMarkers.Enqueue(marker);

                    GameObject tempMarker = Instantiate(readingMarker, hit.point, Quaternion.identity);
                    pathPointTempMarkers.Enqueue(tempMarker);
                    foreach (Transform child in tempMarker.transform)
                    {
                        MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
                        childRenderer.material.color = Color.red;
                    }
                }
            }
        }
    }

    public void triggerClick()
    {
        buttonCounter++;

        // activate setting path
        if (buttonCounter == 1)
        {
            boat.getNextPoint = false;
            pathPoints.Clear();
            camFollowScript.isSetPath = true;
            triggerSettingPathPoints = true;
            ToggleGUI(false);

            gameObject.GetComponentInChildren<Text>().text = "Save Path Points";
        }

        // save changes and reset button
        else if (buttonCounter == 2)
        {
            foreach (var marker in pathPointMarkers)
            {
                Destroy(marker);
            }
            triggerSettingPathPoints = false;
            camFollowScript.isSetPath = false;
            camFollowScript.isEnabled = true;
            buttonCounter = 0;
            ToggleGUI(true);

            gameObject.GetComponentInChildren<Text>().text = "Set Path";

            if (pathPoints.Count != 0)
            {
                gameObject.GetComponent<Button>().interactable = false;
                boat.getNextPoint = true;
            }
        }
    }

    private void ToggleGUI(bool isShown)
    {
        respawnPlume.SetActive(isShown);
        showHidePlume.SetActive(isShown);
        restart.SetActive(isShown);
        exit.SetActive(isShown);
        miniMapFrame.SetActive(isShown);
        infoFrame.SetActive(isShown);
    }
}
