using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPlume : MonoBehaviour
{
    public Queue<GameObject> plumeMarkers = new Queue<GameObject>();
    public GameObject readingMarker;
    public BoatAttack.Boat boat;
    public Plume plumeScript;
    private int buttonCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void triggerClick()
    {
        buttonCounter++;

        // show the plume
        if (buttonCounter == 1)
        {
            plumeMarkers.Clear();
            ShowPlumeOnly();

            gameObject.GetComponentInChildren<Text>().text = "Hide Plume";
        }

        // Clear the plume
        else if (buttonCounter == 2)
        {
            foreach (var marker in plumeMarkers)
            {
                Destroy(marker);
            }
            buttonCounter = 0;

            gameObject.GetComponentInChildren<Text>().text = "Show Plume";
        }
    }

    public void ShowPlumeOnly()
    {
        for (int i = plumeScript.plumeX1; i < plumeScript.plumeX2; i += 5)
        {
            for (int j = plumeScript.plumeY1; j < plumeScript.plumeY2; j += 5)
            {
                GameObject marker = Instantiate(readingMarker, new Vector3(i, 0f, j), Quaternion.identity);
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
