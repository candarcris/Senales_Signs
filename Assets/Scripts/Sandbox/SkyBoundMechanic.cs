using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SkyBoundMechanic : MonoBehaviour
{
    [Header("Ehyal Mecanica de Vuelo")]
    PlayerControllerSigns controllerSigns;
    CharacterController characterController;
    public Ehyal ehyal;
    public float flySpeed = 8f;
    private bool isHangingFromEhyal = false;

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        controllerSigns = GetComponent<PlayerControllerSigns>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- LOGICA DE COLGARSE DE EHYAL ---
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (ehyal != null)
            {
                isHangingFromEhyal = !isHangingFromEhyal;

                if (isHangingFromEhyal)
                {
                    controllerSigns.FrenarEnSeco();
                    controllerSigns.puedeMoverse = false; // Bloquea gravedad y movimiento de Sagar
                    ehyal.isControlledByPlayer = true;
                }
                else
                {
                    ehyal.isControlledByPlayer = false;
                    controllerSigns.puedeMoverse = true; // Devuelve el control a Sagar
                }
            }
            else
            {
                Debug.LogWarning("Falta asignar Ehyal Companion en el inspector (SkyBoundMechanic).");
            }
        }

        if (isHangingFromEhyal)
        {
            // Movimiento 3D libre sin gravedad
            characterController.Move(Vector3.up * 0.05f);
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
            Vector3 moveVelocity = Vector3.zero;

            // Hacia donde mira la camara para moverse segun la perspectiva 3D
            if (dir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                moveVelocity = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * flySpeed;
            }

            // Movimiento Vertical (Arriba / Abajo)
            if (Input.GetButton("Jump")) // Espacio por defecto
            {
                moveVelocity.y = flySpeed;
            }
            else if (Input.GetKey(KeyCode.LeftControl)) // Control para bajar
            {
                moveVelocity.y = -flySpeed;
            }

            // Mover el CharacterController (Mantiene colisiones)
            if (characterController != null)
            {
                characterController.Move(moveVelocity * Time.deltaTime);
            }

            // Giro Visual de Sagar al volar
            if (horizontal != 0)
            {
                float flip = (horizontal > 0) ? 1f : -1f;
                transform.localScale = new Vector3(flip, 1f, 1f);
            }

            return; // Salimos del Update
        }
    }
}
