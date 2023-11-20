using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChangeCollider : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vc;
    [Space]
    [SerializeField] private float _SpeedModifier = 1;


    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.transform.name);
        if(col.CompareTag("Player"))
        {
            _vc.enabled = true;
            CameraController.Instance.SetSpeedModifier(_SpeedModifier);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            _vc.enabled = false;
            CameraController.Instance.SetSpeedModifier(1);
        }
    }
}
