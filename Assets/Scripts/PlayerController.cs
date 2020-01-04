using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Massful massful;

    public float RotationSpeed;
    public float EngineForce;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        massful = GetComponent<Massful>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveForce = Vector3.forward * (Input.GetAxis("Vertical") * EngineForce);
        rigidbody.AddRelativeForce(moveForce);

        var rot = rigidbody.rotation.eulerAngles;
        rot.y += Input.GetAxis("Horizontal") * RotationSpeed;
        rigidbody.MoveRotation(Quaternion.Euler(rot));
    }
}
