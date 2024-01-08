using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFallingIntro : Event
{
    public PlayerController _player;
    public override void ExecuteEvent()
    {
        _player.FallingIntro();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ExecuteEvent();
        }
    }
}
