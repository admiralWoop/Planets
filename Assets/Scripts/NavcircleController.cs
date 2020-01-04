using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavcircleController : MonoBehaviour
{

    public Rigidbody Rigidbody;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Rigidbody.velocity, Vector3.up);
        transform.position = Rigidbody.position;
    }
}
