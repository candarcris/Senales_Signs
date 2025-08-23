using System.Collections.Generic;
using UnityEngine;

public class BeamPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private int poolSize = 15;
    [SerializeField] private Transform poolParent; // Para organizar las balas en jerarquía
    
    private Queue<GameObject> availableBeams = new Queue<GameObject>();
    private List<GameObject> activeBeams = new List<GameObject>();
    
    private void Awake()
    {
        if (poolParent == null)
            poolParent = transform;
            
        InitializePool();
    }
    
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewBeam();
        }
    }
    
    private void CreateNewBeam()
    {
        GameObject beam = Instantiate(beamPrefab, poolParent);
        beam.name = $"Beam_{availableBeams.Count}";
        beam.SetActive(false);
        availableBeams.Enqueue(beam);
    }
    
    public GameObject GetBeam()
    {
        GameObject beam;
        
        if (availableBeams.Count > 0)
        {
            beam = availableBeams.Dequeue();
        }
        else
        {
            // Si no hay beams disponibles, crear uno nuevo
            CreateNewBeam();
            beam = availableBeams.Dequeue();
        }
        
        beam.SetActive(true);
        activeBeams.Add(beam);
        
        return beam;
    }
    
    public void ReturnBeam(GameObject beam)
    {
        if (beam != null && activeBeams.Contains(beam))
        {
            beam.SetActive(false);
            activeBeams.Remove(beam);
            availableBeams.Enqueue(beam);
        }
    }
    
    public void ReturnAllBeams()
    {
        for (int i = activeBeams.Count - 1; i >= 0; i--)
        {
            ReturnBeam(activeBeams[i]);
        }
    }
    
    // Método para obtener estadísticas del pool (útil para debugging)
    public void GetPoolStatus(out int available, out int active)
    {
        available = availableBeams.Count;
        active = activeBeams.Count;
    }

    // Nuevo método para obtener beam con configuración completa
    public GameObject GetBeam(Transform target, Transform ehyalTransform = null)
    {
        GameObject beam = GetBeam();
        
        if (beam != null)
        {
            Beam beamComponent = beam.GetComponent<Beam>();
            if (beamComponent != null)
            {
                // Configurar la bala con el nuevo método SetupBeam
                beamComponent.SetupBeam(target, this, ehyalTransform);
            }
        }
        
        return beam;
    }
}
