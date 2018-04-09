using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 lastFrameMousePostion;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private bool isInvertY;

    void Update()
    {
        if(Input.GetMouseButtonDown(1) )
        {
            lastFrameMousePostion = Input.mousePosition;
        }

        if(Input.GetMouseButton(1))
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastFrameMousePostion;

            transform.Rotate(deltaMousePosition.y * rotationSpeed * ( isInvertY ? -1 : 1), deltaMousePosition.x * rotationSpeed, 0f);
            Vector3 eulerAngle = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerAngle.x, eulerAngle.y, 0f);

            lastFrameMousePostion = Input.mousePosition;
        }
    }
}
