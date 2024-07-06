using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] PowerUp powerUp;

    private void OnTriggerEnter(Collider collider)
    {
        IBuffable buffable;
        if(collider.gameObject.TryGetComponent<IBuffable>(out buffable))
        {
            buffable.Accept(powerUp);
            Destroy(gameObject);
        }
    }
}
