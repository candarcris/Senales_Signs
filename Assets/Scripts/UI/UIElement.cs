using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UIElement : MonoBehaviour
{
    public bool blockState;
    public string id;
    public virtual void Show()
    {
        if (!blockState)
            gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivate UI element
    /// </summary>
    public virtual void Hide()
    {
        if (!blockState)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Remove UI element from hierarchy
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }


}
