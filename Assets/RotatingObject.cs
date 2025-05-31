using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [Header("Object Settings")]
    public GameObject[] objectPrefabs; 
    public int numberOfObjects = 4;
    public float radius = 5f;
    public Vector3 centerPosition = Vector3.zero;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f;
    public bool clockwise = true;
    public Vector3 rotationAxis = Vector3.up; 

    [Header("Visual Effects")]
    public bool showTrail = false;
    public Material trailMaterial;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private float currentAngle = 0f;

    void Start()
    {
        SpawnObjectsAdvanced();
    }

    void Update()
    {
        RotateObjectsAdvanced();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube();
    }
        void SpawnObjectsAdvanced()
    {
        
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        spawnedObjects.Clear();

        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            
            Vector3 localPosition = Vector3.zero;
            if (rotationAxis == Vector3.up)
            {
                localPosition = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            }
            else if (rotationAxis == Vector3.right)
            {
                localPosition = new Vector3(0f, Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            }
            else if (rotationAxis == Vector3.forward)
            {
                localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            }

            Vector3 spawnPosition = centerPosition + localPosition;

            GameObject newObject;
            if (objectPrefabs != null && objectPrefabs.Length > 0)
            {
                GameObject prefab = objectPrefabs[i % objectPrefabs.Length];
                newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newObject.transform.position = spawnPosition;
            }

            
            if (showTrail)
            {
                TrailRenderer trail = newObject.AddComponent<TrailRenderer>();
                trail.time = 2f;
                trail.startWidth = 0.1f;
                trail.endWidth = 0.01f;
                if (trailMaterial != null)
                    trail.material = trailMaterial;
            }

            newObject.name = $"RotatingObject_{i + 1}";
            spawnedObjects.Add(newObject);
        }
    }

    void RotateObjectsAdvanced()
    {
        float speedMultiplier = clockwise ? 1f : -1f;
        currentAngle += rotationSpeed * speedMultiplier * Time.deltaTime;

        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
            {
                float angle = (currentAngle + i * angleStep) * Mathf.Deg2Rad;

                
                Vector3 localPosition = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    centerPosition.z 
                );

                spawnedObjects[i].transform.position = new Vector3(
                    centerPosition.x + localPosition.x,
                    centerPosition.y + localPosition.y,
                    localPosition.z
                );
            }
        }
    }
}
