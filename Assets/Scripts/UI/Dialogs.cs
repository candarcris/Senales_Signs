using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Que no se arrastra a ningun objeto en escena
public class Dialogs
{
    [TextArea(2, 6)]
    public string[] arrayTexts;
}
