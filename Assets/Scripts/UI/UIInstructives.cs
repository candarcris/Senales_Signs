using UnityEngine;

public abstract class UIInstructives : MonoBehaviour
{
    public GameObject _fade;

    private System.Action _onFinishAction;

    public void SetInstruction(bool value)
    {
        if (value)
        {
            if (_onFinishAction == null)
                _onFinishAction = () => SetInstruction(false);

            ManagerLocator.GetDialogsManager().OnFinishDialog -= _onFinishAction; // evita duplicados
            ManagerLocator.GetDialogsManager().OnFinishDialog += _onFinishAction;

            _fade?.SetActive(true);
            gameObject.SetActive(true);

            OnShowInstruction();
        }
        else
        {
            if (_onFinishAction != null)
                ManagerLocator.GetDialogsManager().OnFinishDialog -= _onFinishAction;

            _fade?.SetActive(false);
            gameObject.SetActive(false);

            OnHideInstruction();
        }
    }

    protected virtual void OnShowInstruction() { }
    protected virtual void OnHideInstruction() { }
}

