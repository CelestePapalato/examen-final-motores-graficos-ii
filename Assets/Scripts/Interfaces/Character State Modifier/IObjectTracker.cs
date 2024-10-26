using UnityEngine;

public interface IObjectTracker
{
    public Transform Target { get; set; }

    public void AvoidTransform(Transform transform);

    public void StopAvoiding(Transform transform);
}
