using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BoatAttack
{
    public class Boat : MonoBehaviour
    {
        public Engine engine;
        public Plume plumeScript;
        public GameObject readingMarker;
        public SetPath setPathScript;
        private Transform boatTransform;

        private float O2Value = 0f;
        private Color O2Color;
        public GameObject O2Level;
        public GameObject coordinates;
        public GameObject distance;

        public float x, y, z, prevX, prevY, prevZ = 0;
        private int prevXReading, prevZReading = 0;
        private float travelledDistance = 0f;
        private float readingDistance = 10f;
        private Vector3 newPos;
        private float _targetSide;

        public bool getNextPoint = false;
        private bool pauseBoatMovement = false;
        private int readingDelay = 1;

        private void Awake()
        {
            TryGetComponent(out engine.RB);
        }

        private void Start()
        {
            boatTransform = GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            if (getNextPoint)
            {
                x = boatTransform.position.x;
                y = boatTransform.position.y;
                z = boatTransform.position.z;

                // update travelledDistance
                UpdateDistance();

                prevX = x;
                prevY = y;
                prevZ = z;

                UpdateUI();

                moveToNextPoint();
            }
            else
            {
                newPos = new Vector3(boatTransform.position.x, boatTransform.position.y, boatTransform.position.z);
            }
        }

        private void UpdateDistance()
        {
            if (!pauseBoatMovement)
            {
                float tranvelledX = Mathf.Abs(x - prevX);
                if (tranvelledX >= 20)
                {
                    tranvelledX = 0;
                }

                float tranvelledZ = Mathf.Abs(z - prevZ);
                if (tranvelledZ >= 20)
                {
                    tranvelledZ = 0;
                }

                float travelled = Mathf.Sqrt(Mathf.Pow(tranvelledX, 2) + Mathf.Pow(tranvelledZ, 2));
                travelledDistance += travelled;

                // Debug.Log(tranvelledX.ToString() + " | " + tranvelledZ.ToString() + " | " + travelledDistance.ToString());
            }
        }

        private void UpdateUI()
        {
            O2Level.GetComponent<TextMeshProUGUI>().text = O2Value.ToString("F2") + " mg/l";
            O2Level.GetComponent<TextMeshProUGUI>().color = O2Color;
            coordinates.GetComponent<TextMeshProUGUI>().text = "Coordinates: (x=" + x.ToString("0") + ", y=" + z.ToString("0") + ")";
            distance.GetComponent<TextMeshProUGUI>().text = "Distance Travelled: " + travelledDistance.ToString("0") + " m";
        }

        private void moveToNextPoint()
        {
            Queue<Vector3> pathPoints = setPathScript.pathPoints;

            if ((Mathf.Floor(travelledDistance) % readingDistance) == 0 && !pauseBoatMovement)
            {
                TakeReading();
            }

            if ((Mathf.Abs(x - newPos.x) >= 0) && (Mathf.Abs(x - newPos.x) <= 5f) &&
            (Mathf.Abs(z - newPos.z) >= 0) && (Mathf.Abs(z - newPos.z) <= 5f))
            {
                if (pathPoints.Count != 0)
                {
                    newPos = pathPoints.Dequeue();
                }
                else
                {
                    getNextPoint = false;
                }
            }

            if (!pauseBoatMovement)
            {
                MoveBoat(newPos);
            }
        }

        private void TakeReading()
        {
            int i = (int)gameObject.transform.position.x;
            int j = (int)gameObject.transform.position.z;
            if (!(i == prevXReading && j == prevZReading))
            {
                prevXReading = i;
                prevZReading = j;
                GameObject marker = Instantiate(readingMarker, gameObject.transform.position, Quaternion.identity);
                foreach (Transform child in marker.transform)
                {
                    MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
                    O2Color = getRGB(plumeScript.allValuesZeroToOne[i, j]);
                    childRenderer.material.color = O2Color;
                }
                O2Value = plumeScript.allValues[i, j];
            }
        }

        public Color getRGB(float x)
        {
            float red, green, blue;
            red = green = blue = 0;

            if (x <= 0.5)
            {
                red = (-2f * x + 1);
                green = (2f * x);
                blue = 0;
            }
            else
            {
                red = 0;
                green = (-2f * x + 2);
                blue = (2f * x - 1);
            }

            red *= 255f;
            green *= 255f - 250f;
            blue *= 255f - 250f;
            //Debug.Log(red.ToString() + " | " + green.ToString() + " | " + blue.ToString());

            Color result = new Color((int)red, (int)green, (int)blue);

            return result;
        }

        private void MoveBoat(Vector3 targetDestination)
        {
            // Get angle to the destination and the side
            var normDir = targetDestination - transform.position;
            normDir = normDir.normalized;
            var dot = Vector3.Dot(normDir, transform.forward);
            _targetSide = Vector3.Cross(transform.forward, normDir).y;//positive on right side, negative on left side

            engine.Turn(Mathf.Clamp(_targetSide, -1.0f, 1.0f));
            engine.Accelerate(dot > 0 ? 1f : 0.25f);
        }
    }
}
