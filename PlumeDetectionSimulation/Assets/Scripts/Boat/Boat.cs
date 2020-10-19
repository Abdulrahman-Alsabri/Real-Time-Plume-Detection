using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoatAttack
{
    public class Boat : MonoBehaviour
    {
        public Engine engine;
        private Transform boatTransform;

        public float x, y, z, angle;
        private float _throttle;
        private float _steering;
        private float _targetSide;

        private float steeringTolerance = 2f;

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
            x = boatTransform.position.x;
            y = boatTransform.position.y;
            z = boatTransform.position.z;


            MoveBoat(new Vector3(150f, 0f, 150f));
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
