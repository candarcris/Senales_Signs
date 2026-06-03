using UnityEngine;

namespace Signs
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamage>(out IDamage damage))
            {
                damage.RecibirImpacto(50);
            }
        }
    }
}
