using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorField;

public class GravityMesh : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    Color[] colors; // Add this line
    public GeomContainer geomContainer;
    public AnimationCurve heightCurve; // Add this line
    public Planet planetOne;
    public Rigidbody planetOneRigidbody;
    public Planet planetTwo;
    public Rigidbody planetTwoRigidbody;
    private const float G = 6.674e-11f;
    
    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreatePlaneMesh(100, 100);
        planetOneRigidbody = planetOne.GetComponent<Rigidbody>();
        planetTwoRigidbody = planetTwo.GetComponent<Rigidbody>();
        // UpdateMesh();
        //geomContainer.IntilizeMeshTexturing();
    }

    void Update()
    {
        Debug.Log("update");
        AdjustMeshForGravity();
        mesh.vertices = vertices;
        mesh.RecalculateNormals(); // Recalculate normals to ensure proper lighting
    }

    public void UpdateMesh()
    {
        AdjustMeshForGravity();
        mesh.vertices = vertices;
        mesh.RecalculateNormals(); // Recalculate normals to ensure proper lighting
    }
    
    float maxGravity = 0.0f;

    void AdjustMeshForGravity()
    {
        Color[] colors = new Color[vertices.Length];
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        // Assuming the mesh is positioned at the origin, adjust this as necessary
        Vector3 meshPosition = transform.position;

        List<Tuple<int, float>> forces = new List<Tuple<int, float>>();
        
        // Loop to calculate the gravitational force
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPoint = meshPosition + vertices[i];

            float forceMagnitude1 = CalculateGravitationalForce(worldPoint, planetOne.transform.position, planetOneRigidbody.mass);
            float forceMagnitude2 = CalculateGravitationalForce(worldPoint, planetTwo.transform.position, planetTwoRigidbody.mass);
            
            // Combining forces from both planets. You might need a more sophisticated way to combine these depending on your needs.
            float totalForce = (forceMagnitude1 + forceMagnitude2);

            forces.Add(new Tuple<int, float>(i, totalForce));
        }

        // Sort the forces in descending order
        forces.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        // Map the gravitational force and adjust the vertex Y position
        for (int i = 0; i < forces.Count; i++)
        {
            // Adjust the vertex Y based on the combined gravitational force
            float mappedForce = Mathf.InverseLerp(0, forces.Count, i); // Map the force to a range between 0 and 1
            vertices[forces[i].Item1].y = heightCurve.Evaluate(mappedForce); // Use the curve to adjust the height

            float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[forces[i].Item1].y);
            colors[forces[i].Item1] = Color.Lerp(Color.blue, Color.red, height); // Blue for low, Red for high
        }
        
        
        Vector3 lowestVertex = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            if (vertices[i].y < lowestVertex.y)
            {
                lowestVertex = vertices[i];
            }
        }

        Debug.Log("Lowest vertex position: " + lowestVertex);
        
        mesh.colors = colors; // Apply colors to the mesh
    }


    float CalculateGravitationalForce(Vector3 pointPosition, Vector3 planetPosition, float planetMass)
    {
        float distance = Vector3.Distance(pointPosition, planetPosition);
        return G * planetMass / (distance * distance); // Simplified, adjust as needed
    }

    void CreatePlaneMesh(float size = 10f, int gridSize = 10)
    {
        mesh.Clear();

        int vertCount = (gridSize + 1) * (gridSize + 1); // Total vertices
        int squareCount = gridSize * gridSize; // Total squares
        int triCount = squareCount * 2; // Total triangles (2 per square)
        int triIndexCount = triCount * 3; // Total indices (3 per triangle)

        vertices = new Vector3[vertCount];
        int[] triangles = new int[triIndexCount];
        Vector2[] uv = new Vector2[vertCount]; // For texturing

        // Vertex positions
        int vertIndex = 0;
        float halfSize = size * 0.5f;
        for (int i = 0; i <= gridSize; i++)
        {
            for (int j = 0; j <= gridSize; j++)
            {
                float x = j * (size / gridSize) - halfSize;
                float z = i * (size / gridSize) - halfSize;
                vertices[vertIndex] = new Vector3(x, 0, z);
                uv[vertIndex] = new Vector2((float)j / gridSize, (float)i / gridSize);
                vertIndex++;
            }
        }

        // Triangle indices
        int triIndex = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int cornerIndex = i * (gridSize + 1) + j;
                triangles[triIndex++] = cornerIndex;
                triangles[triIndex++] = cornerIndex + gridSize + 1;
                triangles[triIndex++] = cornerIndex + 1;

                triangles[triIndex++] = cornerIndex + 1;
                triangles[triIndex++] = cornerIndex + gridSize + 1;
                triangles[triIndex++] = cornerIndex + gridSize + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }

}
