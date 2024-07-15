using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Points : MonoBehaviour
{
    public static UnityAction<int> OnCollected;

    [SerializeField] int points;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollected?.Invoke(points);
        Destroy(gameObject);
    }
}
