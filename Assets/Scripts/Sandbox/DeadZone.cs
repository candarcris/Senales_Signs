using UnityEngine;

namespace Signs
{
    public class DeadZone : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null && other.CompareTag("Player")) 
            {
                Debug.Log("Game Over");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
