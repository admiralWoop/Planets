using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetOrbitPlotter : MonoBehaviour
{
    public bool PlotOrbits;
    [Range(1, 240)]
    public float PlotDuration;
    [Range(1f, 50f)]
    public float PlotFrequency;

    public double G;

    private List<Planet> _planets;
    private Dictionary<string, List<(double epoch, Vector3 vector)>> _planetsOrbitPlots;
    private Dictionary<string, LineRenderer> _lineRenderers;

    void Start()
    {
        Initialize();
        if (PlotOrbits) DrawPrediction(0);
    }

    void OnValidate()
    {
        Initialize();
        if (PlotOrbits) DrawPrediction(0);
    }

    void FixedUpdate()
    {
        var time = Time.time;
        if (PlotOrbits && (decimal)time % (1 / (decimal)PlotFrequency) == 0) DrawPrediction(time);
    }

    private void Initialize()
    {
        _planetsOrbitPlots = new Dictionary<string, List<(double epoch, Vector3 vector)>>();
        _planets = FindObjectsOfType<Planet>().Where(p => p.Parent != null).ToList();

        foreach (var planet in _planets)
        {
            _planetsOrbitPlots.Add(planet.name, new List<(double epoch, Vector3 vector)>());
        }


        _lineRenderers = FindObjectsOfType<LineRenderer>()
            .ToDictionary(lr => lr.gameObject.name, lr => lr);

        foreach (var lineRenderer in _lineRenderers.Values)
        {
            lineRenderer.SetPositions(new Vector3[lineRenderer.positionCount]);
        }
    }

    private void DrawPrediction(float time)
    {
        foreach (var planet in _planets)
        {
            var startEpoch = _planetsOrbitPlots[planet.name].Any() ? _planetsOrbitPlots[planet.name].Last().epoch : time;

            if (_planetsOrbitPlots[planet.name].Any())
            {
                foreach (var point in _planetsOrbitPlots[planet.name].Where(pair => pair.epoch < time).ToList())
                {
                    _planetsOrbitPlots[planet.name].Remove(point);
                }
            }

            if (_planetsOrbitPlots[planet.name].Any() &&
                _planetsOrbitPlots[planet.name].Last().epoch > time + PlotDuration) continue;

            foreach (var point in PlotOrbit(planet, startEpoch, time + PlotDuration, 1 / PlotFrequency))
            {
                _planetsOrbitPlots[planet.name].Add(point);
            }
        }

        foreach (var lr in _lineRenderers)
        {
            if (!_planetsOrbitPlots.ContainsKey(lr.Key.Remove(lr.Key.Length - 5))) continue;

            var orbit = _planetsOrbitPlots[lr.Key.Remove(lr.Key.Length - 5)].Select(o => o.vector);
            lr.Value.positionCount = orbit.Count();
            lr.Value.SetPositions(orbit.ToArray());
        }
    }

    private List<(double epoch, Vector3 position)> PlotOrbit(Planet planet, double startEpoch, double endEpoch, double step = 0.02, bool isRelative = false)
    {
        if (step <= 0) throw new ArgumentOutOfRangeException(nameof(step), "Step cannot be less or equal to zero.");

        var verteciesWithEpochs = new List<(double, Vector3)>();

        for (var epoch = startEpoch; epoch <= endEpoch; epoch += step)
        {
            if (planet.Parent != null)
            {
                if (isRelative)
                {
                    verteciesWithEpochs.Add((epoch, PlanetController.GetPosAtEpoch(planet, epoch, G) + planet.Parent.transform.position));
                }
                else
                {
                    verteciesWithEpochs.Add((epoch, PlanetController.GetPosAtEpoch(planet, epoch, G) + PlanetController.GetPosAtEpoch(planet.Parent, epoch, G)));
                }
            }
            else
            {
                verteciesWithEpochs.Add((epoch, PlanetController.GetPosAtEpoch(planet, epoch, G)));
            }
        }

        return verteciesWithEpochs;
    }
}
