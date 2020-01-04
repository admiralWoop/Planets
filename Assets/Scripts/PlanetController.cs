using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class PlanetController : MonoBehaviour
{
    private List<Planet> planets;


    public float G;

    public void OnValidate()
    {
        Initialize();
        PlacePlanetsAtEpoch(0);
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        planets = FindObjectsOfType<Planet>().Where(p => p.Parent != null).ToList();
    }

    void FixedUpdate()
    {
        var time = Time.time;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (time == 0) return;

        PlacePlanetsAtEpoch(time);
    }

    private void PlacePlanetsAtEpoch(float time)
    {
        foreach (var planet in planets)
        {
            planet.transform.position = GetPosAtEpoch(planet, time, G);
            var parentPlanet = planet.Parent;
            while (parentPlanet != null)
            {
                planet.transform.position += GetPosAtEpoch(parentPlanet, time, G);
                parentPlanet = parentPlanet.Parent;
            }
        }
    }

    public static Vector3 GetPosAtEpoch(Planet planet, float time, float G)
    {
        if (planet.Parent == null) return planet.transform.position;

        var ap = planet.Apoapsis;
        var pe = planet.Periapsis;
        var arg = planet.ArgumentOfPeriapsis;

        var a = (ap + pe) / 2; //semi-major axis
        var e = (ap - pe) / (ap + pe);

        //1.Compute the mean anomaly M = nt where n is the mean motion
        //M = nt where n is the mean motion. 
        //n*P=2*Pi where P is the period
        //n=(2*Pi)/P
        var n = Math.Sqrt(G * (planet.Mass + planet.Parent.Mass) / Math.Pow(a, 3)); //mean motion

        var period = 2 * Math.PI * Math.Sqrt(Math.Pow(a, 3) / (G * n));

        time %= (float)period;

        var M = n * time; //mean anomaly
        //2.Compute the eccentric anomaly E by solving Kepler's equation
        var E = CalculateE(e, (float)M);
        //3.Compute the true anomaly v by the equation
        var v = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E / 2));
        v += arg;
        //4.Compute the distance
        var d = a * (1 - e * Math.Cos(E));

        var dir = new Vector3((float)Math.Sin(v), 0, (float)Math.Cos(v));

        return dir * (float)d;
    }

    static float CalculateE(float e, float M)
    {
        var E_ = M;
        float E;

        for (int i = 0; i < 100; i++)
        {
            E = (float)(E_ - (E_ - e * Math.Sin(E_) - M) / (1 - e * Math.Cos((E_))));
            E_ = E;
        }

        return E_;
    }
}