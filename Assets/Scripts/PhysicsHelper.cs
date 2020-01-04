using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class PhysicsHelper
{
    public static Vector3 GetForce(Vector3 massfulPos, double massfulMass, Vector3 PlanetPos, double planetMass, double G)
    {
        var m = massfulMass * planetMass;
        var r = (massfulPos - PlanetPos).sqrMagnitude;
        var F = -(G * (double)(m / r));
        var force = (massfulPos - PlanetPos).normalized * (float)F;
        return force;
    }
}
