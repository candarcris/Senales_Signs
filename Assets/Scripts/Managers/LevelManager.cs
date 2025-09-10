using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyIdentifier
    {
        public string enemyType;
        public Transform spawner;
        public string customName;
    }
    public List<EnemyIdentifier> _enemyIdentifiers = new List<EnemyIdentifier>();
    private Dictionary<Enemy, EnemyIdentifier> _enemyIdentityMap = new Dictionary<Enemy, EnemyIdentifier>();

    public GroundEnemy _enemyPrefab;
    public List<Enemy> _enemyList = new();
    public List<Transform> _enemySpawnerTransformList = new();
    public PlayerController _sagarController;
    public EyalController _ehyalController;
    public Action OnSetEnemies;

    [Header("Level Interactives")]
    public GameObject triggerFirstDialog;
    public GameObject triggerCombatEnemies;
    public GameObject triggerWormAppear;

    private void Start()
    {
        _sagarController.SetFallingDrag(50);
        _sagarController.SetAnimation("FirstFall", true, true);
        StartCoroutine(SetFirstFall());
        if (_enemySpawnerTransformList.Count > 0)
        {
            SetEnemies(_enemySpawnerTransformList[0]);
            SetEnemies(_enemySpawnerTransformList[4]);
        }
        else
        {
            Debug.LogWarning("No hay spawners de enemigos configurados");
        }
    }

    IEnumerator SetFirstFall()
    {
        yield return new WaitForSeconds(5);
        triggerFirstDialog.SetActive(true);
    }

    public void SetEnemies(Transform spawner)
    {
        GroundEnemy newEnemy = new GroundEnemy(10f, 100f);
        GroundEnemy InstanceNewEnemy = Instantiate(_enemyPrefab, spawner.position, spawner.rotation, spawner);
        InstanceNewEnemy._damage = newEnemy._damage;
        InstanceNewEnemy._lifeAmount = newEnemy._lifeAmount;

        EnemyIdentifier identifier = _enemyIdentifiers.Find(id => id.spawner == spawner);

        if (identifier != null)
        {
            InstanceNewEnemy.name = identifier.customName;
            _enemyIdentityMap[InstanceNewEnemy] = identifier;
        }

        // Suscribirse al evento de muerte
        InstanceNewEnemy.OnDeath += () => OnEnemyDeath(InstanceNewEnemy);

        _enemyList.Add(InstanceNewEnemy);
        OnSetEnemies?.Invoke();
    }

    // Agregar este método al LevelManager
    public void SetMultipleEnemies(Transform spawner, int count, float spacing = 2f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = spawner.position + new Vector3(i * spacing, 0, 0);

            // Crear spawner temporal
            GameObject tempSpawner = new GameObject($"TempSpawner_{i}");
            tempSpawner.transform.position = spawnPosition;
            tempSpawner.transform.rotation = spawner.rotation;

            if (_enemySpawnerTransformList.Count > 0)
            {
                SetEnemies(tempSpawner.transform);
            }
            Destroy(tempSpawner);
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        // Identificar por mapeo
        if (_enemyIdentityMap.TryGetValue(enemy, out EnemyIdentifier identifier))
        {
            switch (identifier.customName)
            {
                case "EnemyGround2":
                    _sagarController._canPray = false;
                    _ehyalController.Movement(true);
                    triggerCombatEnemies.SetActive(true);
                    break;
                case "Guardian":
                    
                    break;
                case "KeyEnemy":
                    
                    break;
            }
        }

        // Limpiar del mapeo
        _enemyIdentityMap.Remove(enemy);
    }
}
