using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    public UnityEvent OnClose;
    public UnityEvent OnShow;

    bool isClosed = true;
    public bool Closed { get => isClosed; }

    public virtual void Open()
    {
        isClosed = false;
        OnShow?.Invoke();
    }
    public virtual void Close()
    {
        isClosed = true;
        OnClose?.Invoke();
    }

    public virtual void Initialize() { }
    public virtual void Initialize(bool state) { }
    public virtual void Initialize(object o) { }
}
