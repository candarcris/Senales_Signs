using UnityEngine;
using UnityEngine.InputSystem;


public class InputActions : MonoBehaviour//, Inputs.IPlayerControlActions
{
    //Inputs controls;
    public PlayerController _player;

    public void OnEnable()
    {
        //if (controls == null)
        //{
        //    controls = new Inputs();
        //    controls.PlayerControl.SetCallbacks(this);
        //}
        //controls.PlayerControl.Enable();
    }

    public void OnDisable()
    {
        //controls.PlayerControl.Disable();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        // 'Use' code here.
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {

    }

    public void OnJump(InputAction.CallbackContext context)
    {

    }
}
