using UnityEngine;
using System;
using Cinemachine;

public class CameraViewAdaption : MonoBehaviour
{
    private void Awake()
    {
        var cvc  = GetComponent<CinemachineVirtualCamera>();
        if (cvc)
        {
            cvc.m_Lens.OrthographicSize = cvc.m_Lens.OrthographicSize  * 1080 / 1920 * Screen.height / Screen.width;
        }

        var camera = GetComponent<Camera>();
        if (camera)
        {
            camera.orthographicSize = camera.orthographicSize * 1080 / 1920 * Screen.height / Screen.width;
        }

    }
}