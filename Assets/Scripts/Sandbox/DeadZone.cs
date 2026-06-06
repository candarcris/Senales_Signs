using UnityEngine;

namespace Signs
{
    public class DeadZone : MonoBehaviour
    {
        public int _damage;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamage>(out IDamage damage))
            {
                damage.RecibirImpacto(_damage);
            }
        }
    }
}
