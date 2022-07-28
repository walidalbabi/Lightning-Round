using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_LoadingIcon : MonoBehaviour
{
    private void Start()
    {
        transform.LeanRotateAround(Vector3.forward, -360, 7f).setLoopClamp();
    }
}
