using UnityEngine;
using Cinemachine;

namespace Signs
{
    public class CinematicCameraMoves : MonoBehaviour
    {
        public CinemachineVirtualCamera cmCamera;

        private void Awake()
        {
            cmCamera = GetComponent<CinemachineVirtualCamera>();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        public void ChangeLookAt(Transform target)
        {
            cmCamera.LookAt = target;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
