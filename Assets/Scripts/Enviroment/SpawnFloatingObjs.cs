using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFloatingObjs : MonoBehaviour
{
    public GameObject _floatingPrefab;
    public Transform _objsParent;
    [SerializeField] private List<GameObject> _floatingobjsList = new();
    public int _poolSize = 10;
    public float _spawnInterval = 5.0f;
    private int _currentIndex = 0;
    [SerializeField] private int numberOfActives = 0;
    private float minScaleX = 5f;
    private float maxScaleX = 30f;

    private void Start()
    {
        InitializeObjectPool();

        // Iniciar la rutina de spawn
        StartCoroutine(SpawnObjs());
    }

    private void InitializeObjectPool()
    {
        for(int i = 0; i < _poolSize; i++)
        {
            GameObject newObj = Instantiate(_floatingPrefab, transform.position, Quaternion.identity, _objsParent);

            // Generar valores aleatorios para la escala en cada eje
            float randomScaleX = Random.Range(minScaleX, maxScaleX);

            // Aplicar la escala aleatoria al objeto recién instanciado
            newObj.transform.localScale = new Vector3(randomScaleX, randomScaleX, randomScaleX);

            newObj.SetActive(false);
            _floatingobjsList.Add(newObj);
        }
    }

    private IEnumerator SpawnObjs()
    {
        while (true)
        {
            CheckList();
            // Activar el objeto actual del pool
            GameObject objectToSpawn = _floatingobjsList[_currentIndex];
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = transform.position;

            // Incrementar el índice y volver al inicio si se llega al final
            _currentIndex = (_currentIndex + 1) % _poolSize;

            // Esperar el intervalo de tiempo
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void CheckList()
    {
        if(numberOfActives == _floatingobjsList.Count)
        {
            numberOfActives = 0;
            //foreach (var obj in _floatingobjsList)
            //{
            //    //obj.SetActive(false);
            //}
        }
        numberOfActives += 1;
    }
}
