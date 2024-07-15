using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    [SerializeField] GameObject _objectToEnable;
    [SerializeField] bool _DisableOnStart = true;
    [SerializeField] bool _EnableOnTrigger = true;
    [SerializeField] bool _DisableOnExit = true;

    private void Awake()
    {
        if (!_objectToEnable)
        {
            Destroy(this);
        }
        if (_DisableOnStart)
        {
            _objectToEnable.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_EnableOnTrigger)
        {
            _objectToEnable.SetActive(true);
        }
        else
        {
            _objectToEnable.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_DisableOnExit)
        {
            _objectToEnable.SetActive(false);
        }
        else
        {
            _objectToEnable.SetActive(true);
        }
    }
}
