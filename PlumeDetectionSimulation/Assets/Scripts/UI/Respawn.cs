using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public SetPath setPathsScript;
    public BoatAttack.Boat boat;
    public Plume plumeScript;
    public Button showHidePlume;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (boat.getNextPoint)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void triggerClick()
    {
        if (setPathsScript.pathPoints.Count == 0)
        {
            plumeScript.Respawn();
            if (showHidePlume.GetComponentInChildren<Text>().text.Equals("Hide Plume", StringComparison.InvariantCultureIgnoreCase))
            {
                showHidePlume.onClick.Invoke();
            }
            showHidePlume.onClick.Invoke();
        }
    }
}
