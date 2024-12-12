using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    public Transform Target;
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
            transform.position = Target.position;
        }
        if (rotation && !cameraRotation)
        {
            transform.rotation = Target.rotation;
        }
        if (cameraRotation)
        {
            transform.rotation = cam.transform.rotation;
        }
    }
}
