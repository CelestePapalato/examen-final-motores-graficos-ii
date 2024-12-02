using System;

public interface IObservableVariable
{
    public event Action<int, int> OnUpdate;
    public event Action OnDestroyEvent;
    public int Current { get; }
    public int Max { get; }
}
