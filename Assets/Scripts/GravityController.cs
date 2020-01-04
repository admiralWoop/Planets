using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float G;

    private List<Planet> planets;

    private List<(Transform transform, Massful massful, Rigidbody rigidbody)> objects;
    // Start is called before the first frame update
    void Start()
    {
        objects = FindObjectsOfType<Massful>().Where(m => m.CanBePulled).Select(m => (m.transform, m, m.GetComponent<Rigidbody>())).ToList();
        planets = FindObjectsOfType<Planet>().ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var obj in objects)
        {
            foreach (var planet in planets)
            {
                var force = PhysicsHelper.GetForce(
                    obj.transform.position, 
                    obj.massful.Mass, 
                    planet.gameObject.transform.position, 
                    planet.Mass,
                    G);
                obj.rigidbody.AddForce(force);
            }
        }
    }
}
