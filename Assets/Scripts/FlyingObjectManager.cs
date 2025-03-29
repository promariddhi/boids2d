using UnityEngine;
using System.Collections.Generic;

public class FlyingObjectManager : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float separation_strength = 1.0f;
    [SerializeField] private float cohesion_strength = 1.0f;
    [SerializeField] private float adhesion_strength = 1.5f;
    [SerializeField] private float vision_radius = 3.0f;


    private int maxChildren = 150;

    [SerializeField]
    private GameObject Prefab;

    private List<TriangleScript> boids = new List<TriangleScript>();

    private void Start()
    {
        for(int i = 0; i < maxChildren; i++)
        {
            Vector3 pos = new Vector3(
                    Random.Range(0, Screen.width),
                    Random.Range(0, Screen.height),
                    0
                    );
            Quaternion rot = Quaternion.Euler(
                        0,
                        0,
                        Random.Range(0, 360)
                        );

            Vector3 WorldPos = Camera.main.ScreenToWorldPoint(pos);
            WorldPos.z = 0;

            GameObject boid_obj = Instantiate(
                Prefab,
                WorldPos,
                rot);

            TriangleScript boid = boid_obj.GetComponent<TriangleScript>();
            boid.setParameters(speed, separation_strength, cohesion_strength, adhesion_strength, vision_radius);
            boids.Add(boid);

        }


    }
    private void Update()
    {
       foreach(TriangleScript boid in boids)
        {
            boid.setParameters(speed, separation_strength, cohesion_strength, adhesion_strength, vision_radius);
        }
    }
}
