using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : Item
{
    [SerializeField] PowerUp powerUp;
    [SerializeField] GameObject destroyOnPick;

    private void OnTriggerEnter(Collider collider)
    {
        base.Grab();
        IBuffable[] buffables = collider.gameObject.GetComponentsInChildren<IBuffable>();
        foreach(IBuffable buffable in buffables)
        {
            buffable.Accept(powerUp);
        }

        if(buffables.Length > 0 && destroyOnPick)
        {
            Destroy(destroyOnPick);
        }
    }
}
