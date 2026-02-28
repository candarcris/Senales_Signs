using UnityEngine;

public class PlayerControllerSigns : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        // 1. Obtener inputs
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // 2. Calcular ángulo de movimiento relativo a la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // 3. Mover al personaje
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // 4. Lógica de "Giro" Visual (Flip)
            // Usamos el input horizontal para decidir si el sprite mira a la izq o der
            if (horizontal != 0)
            {
                float flip = (horizontal > 0) ? 1f : -1f;
                transform.localScale = new Vector3(flip, 1f, 1f);
            }
        }

        // Gravedad simple
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
