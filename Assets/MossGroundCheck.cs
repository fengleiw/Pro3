using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGroundCheck : MonoBehaviour
{
    public bool grounded;
    public static MossGroundCheck Instance;
    private void Awake()
    {
        grounded = true;
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
        if (!grounded) { Debug.Log("gr"); }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
