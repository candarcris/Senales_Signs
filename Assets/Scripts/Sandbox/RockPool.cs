using System.Collections.Generic;
using UnityEngine;

public class RockPool : MonoBehaviour
{
    public static RockPool Instance;
    public GameObject rockPrefab;
    public int poolSize = 10;
    
    // 1. Cambiamos List por Stack
    private Stack<GameObject> pool = new Stack<GameObject>();

    void Awake() { Instance = this; }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(rockPrefab);
            obj.SetActive(false);
            pool.Push(obj); // 2. Usamos Push en lugar de Add
        }
    }

    public GameObject GetRock()
    {
        // 3. Ya no hay foreach. Solo revisamos si quedan rocas disponibles.
        if (pool.Count > 0)
        {
            GameObject rock = pool.Pop(); // Saca la roca del Stack al instante (O(1))
            rock.SetActive(true);         // La activamos aquí mismo
            return rock;
        }
        
        // Opcional: Aquí podrías instanciar una nueva roca si te quedaste sin ellas.
        return null; 
    }

    // 4. ¡NUEVO! Con un Stack, necesitas un método explícito para devolver la roca.
    public void ReturnRock(GameObject rock)
    {
        rock.SetActive(false);
        pool.Push(rock); // La metemos de vuelta al Stack
    }
}