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

    private PlayerOrbitPlotter plotter;

    public float RotationSpeed;
    public float EngineForce;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        massful = GetComponent<Massful>();
        plotter = GetComponent<PlayerOrbitPlotter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var moveForce = Vector3.forward * (Input.GetAxis("Vertical") * EngineForce);

        if(moveForce.magnitude > 0) plotter.Replot();

        rigidbody.AddRelativeForce(moveForce);

        var rot = rigidbody.rotation.eulerAngles;
        rot.y += Input.GetAxis("Horizontal") * RotationSpeed;
        rigidbody.MoveRotation(Quaternion.Euler(rot));
    }
}
