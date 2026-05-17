using UnityEngine;
using System.Collections;


public class PrayMechanic : MonoBehaviour
{
    [Header("Mecánica Ehyal")]
    public Transform ehyalTransform; // Arrastra a Ehyal aquí
    public Transform puntoDeDescansoEhyal; // Un objeto vacío flotando sobre el hombro de tu player
    [SerializeField] private bool isEhyalWith = false;
    [SerializeField] private bool canPray = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && canPray)
        {
            isEhyalWith = !isEhyalWith;

            if(isEhyalWith)
            {
                StartCoroutine(RutinaOracion());
            }
        }

        // Si Ehyal fue llamado, hacemos que siga al jugador suavemente
        if (isEhyalWith)
        {
            // Lerp hace un movimiento fluido de persecución
            ehyalTransform.position = Vector3.Lerp(ehyalTransform.position, puntoDeDescansoEhyal.position, Time.deltaTime * 5f);
        }
    }

    private IEnumerator RutinaOracion()
    {
        // 1. Bloqueamos movimiento e iniciamos animación
        // puedeMoverse = false; 
        // animator.SetTrigger("Orar");

        // 2. Ehyal se activa y empieza a volar hacia ti
        //enOracion = true;

        // 3. El tiempo que el jugador debe estar quieto rezando
        yield return new WaitForSeconds(1.5f);

        // 4. Termina la animación, recupera el control, Ehyal se queda siguiéndolo
        // puedeMoverse = true;
    }
}
