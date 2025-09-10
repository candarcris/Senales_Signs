using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class FortressKingEnemy : MonoBehaviour
{
    public Transform _finalPosition;
    private bool _isMoving = true;

    private void Start()
    {
        _isMoving = true;
    }

    public void Move()
    {
        transform.position = Vector3.Lerp(transform.position, _finalPosition.position, Time.deltaTime * 0.2f);
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Move();
        }
    }
}
