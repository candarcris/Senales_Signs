using UnityEngine;

public class Ehyal : MonoBehaviour
{
    public bool isControlledByPlayer = false;
    public Transform playerTarget;
    public Vector3 hangingOffset = new Vector3(0, 1.3f, 0);

    void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    void Update()
    {
        if (isControlledByPlayer)
        {
            // Ehyal se posiciona arriba de Sagar
            if (playerTarget != null)
            {
                transform.position = Vector3.Lerp(transform.position, playerTarget.position + hangingOffset, Time.deltaTime * 15f);
            }
        }

        // Escucha inputs para girar (independientemente si es controlado o si solo estÃ¡ flotando)
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            float flip = (horizontal > 0) ? 0.5f : -0.5f;
            transform.localScale = new Vector3(flip, 0.5f, 0.5f);
        }
    }
}
