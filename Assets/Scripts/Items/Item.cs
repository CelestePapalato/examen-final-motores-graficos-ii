using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] PowerUp powerUp;

    private void OnTriggerEnter(Collider collider)
    {
        IBuffable[] buffables = GetComponentsInChildren<IBuffable>();
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
