using UnityEngine;
using UnityEngine.Events;

namespace Signs
{
    public class EveryEntityHealth : MonoBehaviour, IDamage
    {
        public int maxHealth = 100;
        [SerializeField] private int currentHealth;
        // Estos son eventos que puedes configurar directamente en el Inspector de Unity
        public UnityEvent<int> OnTakeDamage;
        public UnityEvent<int> OnHealthChanged;
        public UnityEvent OnDie;
        void Start()
        {
            currentHealth = maxHealth;
            if (OnHealthChanged != null) OnHealthChanged.Invoke(currentHealth);
        }
        public void RecibirImpacto(int cantidadDaño)
        {
            currentHealth -= cantidadDaño;

            // Ejecutamos el evento. Cualquiera que esté escuchando, reaccionará.
            OnTakeDamage.Invoke(cantidadDaño);
            OnHealthChanged.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                OnDie.Invoke();
            }
        }
    }
}
