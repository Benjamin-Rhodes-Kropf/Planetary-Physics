using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Rigidbody otherPlanet; // Assign in inspector
    public ParticleSystem trailEffect; // Assign in inspector
    private Rigidbody rb; 
    private const float G = 6.674e-11f; // Directly using the calculated value
    public Vector3 intitialVelocity;
    
    private Simulation simulation;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailEffect.Play();
        rb.velocity = intitialVelocity;
    }

    void FixedUpdate()
    {
        Vector3 direction = otherPlanet.position - rb.position;
        float distance = direction.magnitude;
        float forceMagnitude = G * (rb.mass * otherPlanet.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rb.AddForce(force);
    }
}
