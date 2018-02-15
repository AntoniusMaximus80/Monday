using UnityEngine;

// Assignment 1:
// Toni Mutanen, 16TIKOGAME, student ID 1603782
// To calculate the position of the camera, I have to know the distance on z-axis behind the target and the height on the y-axis.
// Time to brush up on my trigonometry!

namespace TankGame
{
    public class CameraFollow : MonoBehaviour, ICameraFollow
    {
        public GameObject _cameraTarget;
        public float _cameraDistance,
            _cameraAngle;
        private float _currentCameraAngle { get; set; }
        private float _currentCameraDistance { get; set; }

        public void SetAngle(float angle)
        {
            // I'm clamping the value between 45 and 80 degrees.
            _currentCameraAngle = Mathf.Clamp(angle, 45f, 80f);
        }

        public void SetDistance(float distance)
        {
            // I'm clamping the value between 4 and 24 Unity units.
            _currentCameraDistance = Mathf.Clamp(distance, 4f, 24f);
        }

        public void SetTarget(Transform targetTransform)
        {
            transform.LookAt(targetTransform);
        }

        /// <summary>
        /// This method positions the camera.
        /// </summary>
        private void PositionCamera()
        {
            // Soh Cah Toa, from basic trigonometry.
            // Soh for getting the Z distance.
            // Sin 30 = x / 12, where x = 12 * (Sin(30)).
            float radianAngle = Mathf.Deg2Rad * _currentCameraAngle;
            transform.position = _cameraTarget.transform.position +
                (-_cameraTarget.transform.forward * _currentCameraDistance * Mathf.Sin(radianAngle)); // Position the camera along the local Z-axis of the target.
            // The height can be solved with cosine.
            transform.position += new Vector3(0f, _currentCameraDistance * Mathf.Cos(radianAngle), 0f); // Offset the camera to the correct height on the Y-axis.
        }

        /// <summary>
        /// Update checks the values each frame for real-time changes.
        /// </summary>
        void Update()
        {
            SetAngle(_cameraAngle);
            SetDistance(_cameraDistance);
            PositionCamera();
            SetTarget(_cameraTarget.transform);
        }
    }
}