using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    bool position;
    [SerializeField]
    bool rotation;
    [SerializeField]
    bool cameraRotation;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(position)
        {
            transform.position = target.position;
        }
        if (rotation && !cameraRotation)
        {
            transform.rotation = target.rotation;
        }
        if (cameraRotation)
        {
            transform.rotation = cam.transform.rotation;
        }
    }
}
