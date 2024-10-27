using UnityEngine;

public interface IAvoidObject
{
    public void AvoidTransform(Transform transform);
    public void StopAvoiding(Transform transform);
}
