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
    
    public GameObject GetBeamBase()
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
    
    // Método para verificar si se puede disparar
    public bool CanShoot()
    {
        return activeBeams.Count < poolSize;
    }

    // Nuevo método para obtener beam con configuración completa
    public GameObject GetBeam(Transform target, Transform ehyalTransform = null, Vector3? direction = null)
    {
        // Verificar si ya hay demasiados beams activos
        if (activeBeams.Count >= poolSize)
        {
            Debug.LogWarning($"Demasiados beams activos ({activeBeams.Count}). Retornando todos al pool.");
            ReturnAllBeams();
            return null; // No disparar hasta que se limpien
        }
        
        GameObject beam = GetBeamBase();
        
        if (beam != null)
        {
            Beam beamComponent = beam.GetComponent<Beam>();
            if (beamComponent != null)
            {
                // Si no hay target pero hay dirección, configurar para disparo direccional
                if (target == null && direction.HasValue)
                {
                    beamComponent.SetupBeamDirectional(this, ehyalTransform, direction.Value);
                }
                else
                {
                    beamComponent.SetupBeam(target, this, ehyalTransform);
                }
            }
        }
        
        return beam;
    }
}
