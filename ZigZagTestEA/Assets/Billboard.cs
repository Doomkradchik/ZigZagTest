using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private BillboardKind _kind;

    public bool lockX;
    public bool lockY;
    public bool lockZ;

    private Vector3 _startEulerAngles;

    public enum BillboardKind
    {
        LookCamera,
        CameraForward
    }

    private void Awake()
    {
        _startEulerAngles = transform.rotation.eulerAngles;
    }

    private void LateUpdate()
    {
        switch (_kind)
        {
            case BillboardKind.LookCamera:
                transform.LookAt(Camera.main.transform.position, Vector3.up);
                break;
            case BillboardKind.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
        }

        var x = lockX ? _startEulerAngles.x : transform.rotation.eulerAngles.x;
        var y = lockY ? _startEulerAngles.y : transform.rotation.eulerAngles.y;
        var z = lockZ ? _startEulerAngles.z : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
    }
}

