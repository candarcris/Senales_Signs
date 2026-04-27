using UnityEngine;

namespace Signs
{
    public class MouseConfigs : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Bloquea el cursor en el centro de la ventana
            Cursor.lockState = CursorLockMode.Locked;

            // Hace que el cursor sea invisible
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
