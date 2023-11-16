using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Transform _UIWindowsParentTransform;

    [Space]

    public Dictionary<string, UIElement> _UIList = new();
    public List<UIData> _UIElementList = new();


    /// <summary>
    /// Registers and instantiate the window to display
    /// </summary>
    /// <param name="content"></param>


    public void InitElementBase(string key, Transform parent)
    {
        if(!_UIList.ContainsKey(key))
        {
            var instance = Instantiate(GetUIElementBaseWindow(key), parent);
            RegisterUIWindow(key, instance);
        }
        else
        {
            GetUIElementWindow(key).Show();
        }
    }

    /// <summary>
    /// Registers the instantiated UI element window using a key and the instance
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element"></param>
    public void RegisterUIWindow(string key, UIElement element)
    {
        _UIList.Add(key, element);
    }

    public void UnRegisterUIWindow(string key)
    {
        _UIList.Remove(key);
    }

    /// <summary>
    /// Get the attached UIElement window base prefab using a key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public UIElement GetUIElementBaseWindow(string key)
    {
        foreach (var UIData in _UIElementList)
        {
            if (UIData._key == key)
            {
                return UIData._UIElement;
            }
        }
        return null;
    }

    /// <summary>
    /// Get the Instanced UI element window using a key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public UIElement GetUIElementWindow(string key)
    {
        if (_UIList.ContainsKey(key))
        {
            return _UIList[key];
        }
        return null;
    }
}
