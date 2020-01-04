using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Massful : MonoBehaviour
{
    public double Mass => GetComponent<Rigidbody>().mass;
    public bool CanBePulled;
}
