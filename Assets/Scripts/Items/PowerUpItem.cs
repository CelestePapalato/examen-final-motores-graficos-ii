using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [SerializeField] PowerUp powerUp;

    private void OnTriggerEnter(Collider collider)
    {
        IBuffable[] buffables = collider.gameObject.GetComponentsInChildren<IBuffable>();
        foreach(IBuffable buffable in buffables)
        {
            buffable.Accept(powerUp);
        }

        if(buffables.Length > 0)
        {
            Destroy(gameObject);
        }
    }
}
