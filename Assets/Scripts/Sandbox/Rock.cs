using UnityEngine;

public class Rock : MonoBehaviour
{
    private float deactivateTime;
    public void DeactiveRock()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerSigns playerScript = other.gameObject.GetComponent<PlayerControllerSigns>();
            if (playerScript != null)
            {
                playerScript.RecibirImpacto();
            }
            DeactiveRock();
        }
        else if (other.CompareTag("Floor"))
        {
            DeactiveRock();
        }
    }

    //private void Update()
    //{
    //    if(Time.time >= deactivateTime)
    //    {
    //        DeactiveRock();
    //        deactivateTime = Time.time + 6f;
    //    }
    //}
}
