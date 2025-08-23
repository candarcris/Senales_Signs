using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerCombatEnemies : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;
    [SerializeField] private LevelManager _levelManager;
    public CinemachineVirtualCamera _cinemaCamera;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _levelManager = ManagerLocator.GetLevelManager();

    }


    void AtFinalDialogs()
    {
        _playerController._canPray = true;
        _boxCollider.enabled = false;
        //_levelManager.SetMultipleEnemies(_levelManager._enemySpawnerTransformList[2], 6, 2f);

        for(var i = 2; i <= 6; i++)
        {
            if (_levelManager._enemySpawnerTransformList.Count > 0)
            {
                _levelManager.SetEnemies(_levelManager._enemySpawnerTransformList[i]);
            }
        }
    }

    protected override void DoTriggerEvent()
    {
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += AtFinalDialogs;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);

        //StartCoroutine(CinematicShowEnemy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }

    IEnumerator CinematicShowEnemy()
    {
        _playerController.StopState();
        if (_levelManager._enemyList.Count > 0)
        {
            // Encontrar el enemigo más cercano al PlayerController
            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Enemy enemy in _levelManager._enemyList)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    float distance = Vector3.Distance(_playerController.transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy != null)
            {
                _cinemaCamera.Follow = closestEnemy.transform;
            }
        }
        gameObject.SetActive(false);

        yield return new WaitForSeconds(3);

        _cinemaCamera.Follow = _playerController.transform;
        _playerController._sePuedeMover = true;
    }
}
