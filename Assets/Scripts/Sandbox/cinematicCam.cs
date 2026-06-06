using UnityEngine;
using System.Collections;
using Cinemachine;

namespace Signs
{
    public class cinematicCam : MonoBehaviour
    {
        public int activeTime;

        private void Start()
        {
            StartCoroutine(ActiveCamRoutine());
        }

        private IEnumerator ActiveCamRoutine()
        {
            yield return new WaitForSeconds(activeTime);

            this.gameObject.SetActive(false);
        }
    }
}
