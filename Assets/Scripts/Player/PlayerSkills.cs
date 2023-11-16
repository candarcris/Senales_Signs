using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    PlayerController _playerController;
    [Header("Animacion")]
    private Animator _anim;
    private Rigidbody _rb2D;

    [Header("Habilidades")]
    public GameObject _dialogUI;
    public List<GameObject> _eventsTrigerList = new();
    public bool _isRizpaForm = false;

    [Header("Damage and Die")]
    public bool _inDamage;
    [SerializeField] private float _tiempoPerdidaControl;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
    }

    public void Talk(string msg)
    {

    }

    public void EnterRizpaForm()
    {
        if (_isRizpaForm == true)
        {
            _anim.SetLayerWeight(0, 0f);
            _anim.SetLayerWeight(1, 0f);
            _anim.SetLayerWeight(2, 1f);
        }
    }

    public void Die()
    {
        GameManager._sharedInstance.GameOver();
        //Animacion de muerte
    }

    public void Damage(Vector3 posicion)
    {
        _anim.SetTrigger("InDamage");
        StartCoroutine(WaitForDamage());
        _playerController.Rebote(posicion);

    }

    private IEnumerator WaitForDamage()
    {
        _playerController._sePuedeMover = false;
        yield return new WaitForSeconds(_tiempoPerdidaControl);
        _playerController._sePuedeMover = true;
    }

    private void FixedUpdate()
    {
        //Damage();
    }
}
