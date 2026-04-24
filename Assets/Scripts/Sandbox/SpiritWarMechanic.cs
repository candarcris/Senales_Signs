using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpiritWarMechanic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float repelRadius = 5f;
    [SerializeField] float repelForce = 10f;

    [SerializeField] int enemiesNearbyCount = 0;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            enemiesNearbyCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesNearbyCount--;
        }
    }

    void Shock()
    {
        // Find all colliders within the repel radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, repelRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                if (hit.TryGetComponent(out Rigidbody enemyRb))
                {
                    if (hit.TryGetComponent(out NavMeshAgent agent) && agent.enabled)
                    {
                        StartCoroutine(ShockEnemy(agent, enemyRb));
                    }
                    else
                    {
                        enemyRb.isKinematic = false;
                    }
                }
            }
        }
    }

    public IEnumerator ShockEnemy(NavMeshAgent enemy, Rigidbody rb)
    {
        enemy.enabled = false;
        yield return new WaitForSeconds(3f);
        enemy.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && enemiesNearbyCount > 0)
        {
            Shock();
        }
    }
}
