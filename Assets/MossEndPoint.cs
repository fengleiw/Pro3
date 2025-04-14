using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossEndPoint : MonoBehaviour
{
    [SerializeField] GameObject leftPoint;
    [SerializeField] GameObject rightPoint; 
    public bool mossDetected;

    public static MossEndPoint Instance;
    private void Awake()
    {
        // Check if Instance is already set
        if (Instance == null)
        {
            // Set the Instance to this instance of SpawnMoss
            Instance = this;
            // Optionally, make this object persistent across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one to enforce the singleton pattern
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Moss"))
        {
            mossDetected = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Moss"))
        {
            mossDetected = false;
        }
    }

}


