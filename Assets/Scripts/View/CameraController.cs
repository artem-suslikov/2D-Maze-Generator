using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    public void SetCamera(int width, int height)
    {
        if ((float)(width / height) > 1.9)
        {
            cam.orthographicSize = ((float)width / 2);
        }
        else
        {
            cam.orthographicSize = ((float)height / 2) + 1;
        }
        
        cam.transform.SetPositionAndRotation(new Vector3(((float)width / 2) - 0.5f, ((float)height / 2) - 1.0f, -10), Quaternion.identity);
    }
}
