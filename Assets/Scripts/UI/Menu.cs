using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    public UnityEvent OnClose;
    public UnityEvent OnShow;

    public virtual void Open()
    {
        OnShow?.Invoke();
    }
    public virtual void Close()
    {
        OnClose?.Invoke();
    }
    public virtual void Initialize() { }
    public virtual void Initialize(bool state) { }
    public virtual void Initialize(object o) { }
}
