using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ScaleOverTime : MonoBehaviour
{

    [SerializeField] private float _time;
    [SerializeField] private Vector3 _targetScale;
   
    void OnEnable()
    {
        StartScale();
    }


    private void StartScale()
    {
        transform.LeanScale(_targetScale, _time);
    }


    public void ResetScaleToZero()
    {
        transform.LeanScale(Vector3.zero, _time);
    }

}
