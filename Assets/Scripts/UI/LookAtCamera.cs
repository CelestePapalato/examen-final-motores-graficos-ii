using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] float tolerance = 10f;
    RectTransform canvas;
    Camera cam;

    private void Start()
    {
        canvas = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (IsVisible())
        {
            transform.LookAt(cam.transform);
        }
    }

    private bool IsVisible()
    {
        Vector3 screenPoint = cam.WorldToScreenPoint(canvas.position);
        return ((screenPoint.x >= 0 - tolerance && screenPoint.x <= Screen.width + tolerance)
            && screenPoint.y >= 0 - tolerance && screenPoint.y <= Screen.height + tolerance);
    }
}
