using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstructiveCombat : UIInstructives
{
    public BoxCollider _triggerEventCollider;
    private LevelManager _levelManager;
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;

    private void Awake()
    {
        _levelManager = ManagerLocator.GetLevelManager();
    }
    public void DoDialog()
    {
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
    }

    protected override void OnShowInstruction()
    {
        DoDialog();
    }

    protected override void OnHideInstruction()
    {
        if(_levelManager._enemySpawnerTransformList.Count > 0)
        {
            _levelManager.SetEnemies(_levelManager._enemySpawnerTransformList[1]);
        }
        _triggerEventCollider.gameObject.SetActive(true);
    }
}
