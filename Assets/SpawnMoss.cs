using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMoss : MonoBehaviour
{
    
    [SerializeField] Transform leftSpawnPoint;
    [SerializeField] Transform rightSpawnPoint;
    [SerializeField] GameObject Moss;
    public bool right;
    public bool detected;
    
    

    public static SpawnMoss Instance;
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

    
    private void Update()
    {
        if (detected && GameObject.FindWithTag("Moss") == null)
        {
            if (right)
            {
                Instantiate(Moss, rightSpawnPoint.position, Quaternion.identity);
            }
            else
            {
                Instantiate(Moss, leftSpawnPoint.position, Quaternion.identity);
            }
            
        }
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detected = false;
        }
    }


}
