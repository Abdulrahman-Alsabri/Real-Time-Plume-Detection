using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SetPath : MonoBehaviour
{

    public CameraFollow camFollowScript;
    public BoatAttack.Boat boat;
    public Plume plumeScript;
    public GameObject pathPointMarker;
    public GameObject readingMarker;
    public GameObject plumePositionText;
    public GameObject plumeShapeText;
    public GameObject plumeOrientationText;
    public GameObject plumeRandomizationText;

    public GameObject respawnPlume;
    public GameObject showHidePlume;
    public GameObject restart;
    public GameObject exit;
    public GameObject miniMapFrame;
    public GameObject infoFrame;

    public Queue<Vector3> pathPoints = new Queue<Vector3>();
    public Queue<GameObject> pathPointMarkers = new Queue<GameObject>();
    public Queue<GameObject> pathPointTempMarkers = new Queue<GameObject>();
    public Queue<GameObject> originalPlumeMarkers = new Queue<GameObject>();
    public Queue<GameObject> plumeMarkers = new Queue<GameObject>();

    private int buttonCounter = 0;
    private bool triggerSettingPathPoints = false;
    private bool triggerPlumeSpawning = false;
    private bool isValidSpawnPos = false;
    private bool isPlumeCreated = false;
    private int plumeIndex, orientation, plumeX, plumeY, plumeMidX, plumeMidY = 0;


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
        else if (triggerPlumeSpawning && !boat.getNextPoint)
        {
            ProjectPlume();

            if (Input.GetButton("N"))
            {
                orientation = 0;
                plumeIndex++;
                if (plumeIndex >= plumeScript.numberOfPlumes)
                {
                    plumeIndex = 0;
                }

                if (plumeX == 0 || plumeY == 0)
                {
                    plumeX = 150 - 75;
                    plumeY = 150 - 75;
                }

                isValidSpawnPos = true;

                ProjectPlumeHelper();
            }

            if (Input.GetButton("O"))
            {
                orientation++;
                if (orientation >= 4)
                {
                    orientation = 0;
                }

                if (plumeX == 0 || plumeY == 0)
                {
                    plumeX = 150 - 75;
                    plumeY = 150 - 75;
                }

                isValidSpawnPos = true;

                ProjectPlumeHelper();
            }

            if (Input.GetButton("R"))
            {
                isValidSpawnPos = true;

                foreach (var marker in plumeMarkers)
                {
                    Destroy(marker);
                }

                plumeScript.SpawnPlume(true, true);

                isPlumeCreated = false;
                isValidSpawnPos = false;
            }

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
                    if (!isValidSpawnPos)
                    {
                        int halfPlume = 75;
                        plumeMidX = (int)clickPosition.x;
                        plumeMidY = (int)clickPosition.z;

                        if (plumeScript.SpawnValidity(plumeMidX - halfPlume, plumeMidY - halfPlume))
                        {
                            plumeX = plumeMidX - halfPlume;
                            plumeY = plumeMidY - halfPlume;
                            isValidSpawnPos = true;
                        }
                        else if (plumeScript.SpawnValidity(plumeMidX - halfPlume, plumeMidY + halfPlume))
                        {
                            plumeX = plumeMidX - halfPlume;
                            plumeY = plumeMidY + halfPlume;

                            isValidSpawnPos = true;
                        }
                        else if (plumeScript.SpawnValidity(plumeMidX + halfPlume, plumeMidY - halfPlume))
                        {
                            plumeX = plumeMidX + halfPlume;
                            plumeY = plumeMidY - halfPlume;

                            isValidSpawnPos = true;
                        }
                        else if (plumeScript.SpawnValidity(plumeMidX + halfPlume, plumeMidY + halfPlume))
                        {
                            plumeX = plumeMidX + halfPlume;
                            plumeY = plumeMidY + halfPlume;

                            isValidSpawnPos = true;
                        }
                    }


                }

                ProjectPlumeHelper();
            }
        }
    }

    private void ProjectPlumeHelper()
    {
        if (isValidSpawnPos)
        {

            foreach (var marker in plumeMarkers)
            {
                Destroy(marker);
            }
            plumeScript.SpawnPlume(false, false, plumeIndex, orientation, plumeX, plumeY);
            isPlumeCreated = false;
            isValidSpawnPos = false;

            /*
            */
        }
        else
        {

        }
    }

    public void ProjectPlume()
    {
        if (!isPlumeCreated)
        {
            isPlumeCreated = true;
            for (int i = plumeScript.plumeX1; i < plumeScript.plumeX2; i += 5)
            {
                for (int j = plumeScript.plumeY1; j < plumeScript.plumeY2; j += 5)
                {
                    GameObject marker = Instantiate(pathPointMarker, new Vector3(i, 0f, j), Quaternion.identity);
                    foreach (Transform child in marker.transform)
                    {
                        MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
                        childRenderer.material.color = boat.getRGB(plumeScript.allValuesZeroToOne[i, j]);
                    }
                    plumeMarkers.Enqueue(marker);
                }
            }
        }
    }

    public void triggerClick()
    {
        buttonCounter++;

        // set plume position
        if (buttonCounter == 1)
        {
            boat.getNextPoint = false;
            pathPoints.Clear();
            camFollowScript.isSetPath = true;
            triggerSettingPathPoints = false;
            triggerPlumeSpawning = true;
            ToggleGUI(false);

            plumePositionText.SetActive(true);
            plumeShapeText.SetActive(true);
            plumeOrientationText.SetActive(true);
            plumeRandomizationText.SetActive(true);
            plumePositionText.GetComponent<TextMeshProUGUI>().text = "Use Left Mouse Button to Snap a Plume on a specific Point";
            plumeShapeText.GetComponent<TextMeshProUGUI>().text = "Press 'N' To Get Plume's Next Shape";
            plumeOrientationText.GetComponent<TextMeshProUGUI>().text = "Press 'O' To Change Plume's Orientation";
            plumeRandomizationText.GetComponent<TextMeshProUGUI>().text = "Press 'R' To Randomize The Plume";
            gameObject.GetComponentInChildren<Text>().text = "Save Plume Settings";
        }

        // activate setting path
        if (buttonCounter == 2)
        {
            boat.getNextPoint = false;
            pathPoints.Clear();
            camFollowScript.isSetPath = true;
            triggerSettingPathPoints = true;
            triggerPlumeSpawning = false;
            ToggleGUI(false);

            foreach (var marker in plumeMarkers)
            {
                Destroy(marker);
            }

            plumePositionText.GetComponent<TextMeshProUGUI>().text = "";
            plumeShapeText.GetComponent<TextMeshProUGUI>().text = "";
            plumeOrientationText.GetComponent<TextMeshProUGUI>().text = "Define The Path Points For The Boat To Follow";
            plumeRandomizationText.GetComponent<TextMeshProUGUI>().text = "";
            gameObject.GetComponentInChildren<Text>().text = "Save Path Points";
        }

        // save changes and reset button
        else if (buttonCounter == 3)
        {
            foreach (var marker in pathPointMarkers)
            {
                Destroy(marker);
            }
            isPlumeCreated = false;
            triggerSettingPathPoints = false;
            triggerPlumeSpawning = false;
            camFollowScript.isSetPath = false;
            camFollowScript.isEnabled = true;
            buttonCounter = 0;
            ToggleGUI(true);

            plumePositionText.GetComponent<TextMeshProUGUI>().text = "";
            plumeShapeText.GetComponent<TextMeshProUGUI>().text = "";
            plumeOrientationText.GetComponent<TextMeshProUGUI>().text = "";
            plumeRandomizationText.GetComponent<TextMeshProUGUI>().text = "";

            plumePositionText.SetActive(false);
            plumeShapeText.SetActive(false);
            plumeOrientationText.SetActive(false);
            plumeRandomizationText.SetActive(false);

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
