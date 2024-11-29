using UnityEngine;
using UnityEngine.Events;

public abstract class Item : MonoBehaviour
{
    public UnityEvent OnPickUp;

    public virtual void Grab()
    {
        OnPickUp?.Invoke();
    }
}
