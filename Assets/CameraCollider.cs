using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    
   public static bool OnCollid = false;
    private void OnCollisionStay(Collision collision)
    {
        OnCollid = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        OnCollid = false;
    }
    [SerializeField]
    public bool boo;

    private void Update()
    {
        boo = OnCollid;
    }
}
