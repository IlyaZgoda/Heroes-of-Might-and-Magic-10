using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets._Project.Code
{
    public class Indicator : MonoBehaviour
    {
        public static Indicator Instance;

        private void Awake()
        {
            Instance = this;
        }

        private Transform _following;

        private void LateUpdate()
        {
            if (_following == null)
            {
                return;
            }

            Vector3 position =  GetFollowingPointPosition();

            transform.position = position;
        }

        public void Follow(GameObject following) =>
            _following = following.transform;

        private Vector3 GetFollowingPointPosition()
        {
            Vector3 followingPosition = _following.position;
            followingPosition.y += 1f;

            return followingPosition;
        }
    }
}
