using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerCombatEvent : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;
    private LevelManager _levelManager;
    public CinemachineVirtualCamera _cinemaCamera;
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _levelManager = ManagerLocator.GetLevelManager();
    }

    protected override void DoTriggerEvent()
    {
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        StartCoroutine(CinematicShowEnemy());
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

        yield return new WaitForSeconds(2);

        _cinemaCamera.Follow = _playerController.transform;
        _playerController._sePuedeMover = true;
        _boxCollider.enabled = false;
    }
}
