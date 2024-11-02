using System;

public interface IObservableVariable
{
    public event Action<int, int> OnUpdate;
    public int Current { get; }
    public int Max { get; }
}
