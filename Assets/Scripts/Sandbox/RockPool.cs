using System.Collections.Generic;
using UnityEngine;

public class RockPool : MonoBehaviour
{
    public static RockPool Instance;
    public GameObject rockPrefab;
    public int poolSize = 10;
    private List<GameObject> pool = new List<GameObject>();

    void Awake() { Instance = this; }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(rockPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetRock()
    {
        foreach (GameObject rock in pool)
        {
            if (!rock.activeInHierarchy) return rock;
        }
        return null;
    }
}
