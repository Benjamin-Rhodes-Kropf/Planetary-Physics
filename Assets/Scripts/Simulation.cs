using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Simulation : MonoBehaviour
{
    private static double G = 6.67430e-11;
    private static double distance_between_earth_and_sun = 1.496e11;
    
    private Body earth = new Body();
    private Body sun = new Body();
    private Body moon = new Body();
    
    [SerializeField] private float time_scale = 1;
    
    
    public GameObject earthObject;
    public GameObject sunObject;
    // public GameObject moonObject;
    
    // Start is called before the first frame update
    void Awake()
    {
        sun.Mass = 1.989e30;
        sun.Position = new Vector3D(0, 0, 0); // Sun is at the center
        sun.Velocity = new Vector3D(0, 0, 0); // Sun is not moving
        sun.transform = sunObject.transform;
        
        
        double eccentricity = 0.0167086; // Eccentricity of Earth's orbit
        double semiMajorAxis = 1.496e11; // Semi-major axis in meters
        double velocityAtPerihelion = Math.Sqrt(G * sun.Mass * (1 + eccentricity) / semiMajorAxis);
        double perihelionDistance = semiMajorAxis * (1 - eccentricity); // Distance at perihelion in meters

        earth.Mass = 5.972e24; // Mass of Earth in kg
        earth.Position = new Vector3D(0, 0, perihelionDistance); // Earth is 1 AU away from the Sun 1.496e11
        //Math.Sqrt(G * sun.Mass / distance_between_earth_and_sun); // Velocity for stable orbit
        earth.Velocity = new Vector3D(velocityAtPerihelion, 0, 0); 
        earth.transform = earthObject.transform;
        
        // moon.Mass = 7.342e22; // Mass of Moon in kg
        // moon.Position = new Vector3D(1.496e11 + 3.844e8, 0, 0); // Moon is 384,400 km away from the Earth
        // moon.Velocity = new Vector3D(0, Math.Sqrt(G * earth.Mass / (distance_between_earth_and_sun + 3.844e8)), 0); // Velocity for stable orbit
        // moon.transform = moonObject.transform;
        
        //debug velocities
        Debug.Log("Init Earth Velocity: " + earth.Velocity.X + ", " + earth.Velocity.Y + ", " + earth.Velocity.Z);
        Debug.Log("Init Sun Velocity: " + sun.Velocity.X + ", " + sun.Velocity.Y + ", " + sun.Velocity.Z);
    }

    void UpdatePositionOfPlanets()
    {
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        double dt = Time.deltaTime * time_scale; // Time step

        // Calculate the vector from the earth to the sun
        Vector3D r = new Vector3D(sun.Position.X - earth.Position.X, sun.Position.Y - earth.Position.Y, sun.Position.Z - earth.Position.Z);

        // Calculate the distance between the earth and the sun
        double distance = Math.Sqrt(r.X * r.X + r.Y * r.Y + r.Z * r.Z);

        // Calculate the force of gravity
        double F = G * earth.Mass * sun.Mass / (distance * distance);

        // Calculate the acceleration of the earth
        Vector3D a_earth = new Vector3D(F * r.X / (earth.Mass * distance), F * r.Y / (earth.Mass * distance), F * r.Z / (earth.Mass * distance));

        // Update the velocity of the earth
        earth.Velocity = new Vector3D(earth.Velocity.X + a_earth.X * dt, earth.Velocity.Y + a_earth.Y * dt, earth.Velocity.Z + a_earth.Z * dt);

        // Update the position of the earth
        earth.Position = new Vector3D(earth.Position.X + earth.Velocity.X * dt, earth.Position.Y + earth.Velocity.Y * dt, earth.Position.Z + earth.Velocity.Z * dt);

        //update transforms but scale them down significatly
        earth.transform.position = new Vector3((float)earth.Position.X / 1e9f, (float)earth.Position.Y / 1e9f, (float)earth.Position.Z / 1e9f);
        sun.transform.position = new Vector3((float)sun.Position.X / 1e9f, (float)sun.Position.Y / 1e9f, (float)sun.Position.Z / 1e9f);
        
        //debug positions
        Debug.Log("Earth Position: " + earth.Position.X + ", " + earth.Position.Y + ", " + earth.Position.Z);
        Debug.Log("Sun Position: " + sun.Position.X + ", " + sun.Position.Y + ", " + sun.Position.Z);
        
        //debug velocities
        Debug.Log("Earth Velocity: " + earth.Velocity.X + ", " + earth.Velocity.Y + ", " + earth.Velocity.Z);
        Debug.Log("Sun Velocity: " + sun.Velocity.X + ", " + sun.Velocity.Y + ", " + sun.Velocity.Z);
    }
    
    public class Body
    {
        public double Mass { get; set; }
        public Vector3D Velocity { get; set; }
        public Vector3D Position { get; set; }
        public Transform transform { get; set; }
    }
    
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
