﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerOrbitPlotter : MonoBehaviour
{
    private List<Planet> planets;
    [SerializeField]
    private GameObject player;
    private Rigidbody playerRigidbody;
    [SerializeField]
    private LineRenderer lineRenderer;
    private List<(float epoch, Vector3 position, Vector3 velocity)> plottedOrbit;

    public bool EnablePlotting;
    [Range(1, 240)]
    public float PlotDuration;
    [Range(1f, 50f)]
    public float PlotFrequency;
    [Range(0.02f, 1f)]
    public float PlotDetail;

    public float G;

    void Start()
    {
        Initialize();
        if (EnablePlotting) DrawPlot(0);
    }

    void OnValidate()
    {
        Initialize();
        if (EnablePlotting) DrawPlot(0);
    }

    void FixedUpdate()
    {
        var time = Time.time;
        if (EnablePlotting && (decimal)time % (1 / (decimal)PlotFrequency) == 0) DrawPlot(time);
    }

    void Initialize()
    {
        playerRigidbody = player.GetComponent<Rigidbody>();
        planets = FindObjectsOfType<Planet>().ToList();
        plottedOrbit = new List<(float epoch, Vector3 position, Vector3 velocity)>();

        lineRenderer.SetPositions(new Vector3[lineRenderer.positionCount]);
    }

    void DrawPlot(float time)
    {
        var startEpoch = plottedOrbit.Any() ? plottedOrbit.Last().epoch : time;
        var startPos = plottedOrbit.Any() ? plottedOrbit.Last().position : playerRigidbody.position;
        var startVel = plottedOrbit.Any() ? plottedOrbit.Last().velocity : playerRigidbody.velocity;

        if (startEpoch > time + PlotDuration) return;

        if (plottedOrbit.Any())
        {
            foreach (var point in plottedOrbit.Where(pair => pair.epoch <= time).ToList())
            {
                plottedOrbit.Remove(point);
            }
        }

        plottedOrbit.AddRange(PlotOrbit(startPos, startVel, startEpoch, startEpoch + PlotDuration, PlotDetail));

        lineRenderer.positionCount = plottedOrbit.Count();
        lineRenderer.SetPositions(plottedOrbit.Select(o => o.position).ToArray());
    }

    public List<(float epoch, Vector3 position, Vector3 velocity)> PlotOrbit(Vector3 playerPosAtStart, Vector3 playerVelAtStart, float startEpoch, float endEpoch, float step)
    {
        Debug.Log("Plotting from " + startEpoch + " to " + endEpoch);

        if (step <= 0) throw new ArgumentOutOfRangeException(nameof(step), "Step cannot be less or equal to zero.");

        var playerMass = playerRigidbody.mass;
        var plot = new List<(float epoch, Vector3 position, Vector3 velocity)>();

        var playerVel = playerVelAtStart;
        var playerPos = playerPosAtStart;

        for (var t = startEpoch + step; t <= endEpoch; t += step)
        {
            if (planets.Select(planet => PlanetController.GetPosAtEpoch(planet, t, G)).Any(p => (playerPos - p).magnitude < 3)) break; //break if we got too close to a planet

            var sF = planets
                .Select(planet => PhysicsHelper.GetForce(
                    playerPos,
                    playerMass,
                    PlanetController.GetPosAtEpoch(planet, t, G),
                    planet.Mass,
                    G))
                .Aggregate((Vector3 a, Vector3 b) => a + b); //ΣF - sum of all forces
            playerVel += (sF / playerMass) * (float)step;
            playerPos += playerVel * (float)step;
            plot.Add((t, playerPos, playerVel));
        }
        return plot;
    }

    public void Replot()
    {
        plottedOrbit.Clear();
    }
}
