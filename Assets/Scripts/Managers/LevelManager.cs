using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GroundEnemy _enemyPrefab;
    public List<Enemy> _enemyList = new();
    public Transform _enemySpawner;


    private void Start()
    {
        SetEnemies();
    }
    public void SetEnemies()
    {
        GroundEnemy newEnemy = new GroundEnemy(10f, 100f);
        GroundEnemy InstanceNewEnemy = Instantiate(_enemyPrefab, _enemySpawner.position, _enemySpawner.rotation, _enemySpawner);
        InstanceNewEnemy._damage = newEnemy._damage;
        InstanceNewEnemy._lifeAmount = newEnemy._lifeAmount;
        _enemyList.Add(InstanceNewEnemy);
    }
}
