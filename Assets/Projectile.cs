using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] public string targetTag = "Player";

    [Header("Movement Settings")]

    [SerializeField] public float speed = 5.0f;

    [Tooltip("Maximum acceleration to reach target speed")]
    [SerializeField] public float acceleration = 10.0f;

    [SerializeField] public float stoppingDistance = 0.5f;
    [SerializeField]  public float rotationSpeed = 5.0f;

    [SerializeField]  private Transform target;
    [SerializeField] private Vector3 currentVelocity;

    void Start()
    {
        // Đảm bảo đạn có Collider2D và Rigidbody2D
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Set là trigger để không có va chạm vật lý
        }

        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // Tắt trọng lực
            rb.isKinematic = true; // Set là kinematic để không bị ảnh hưởng bởi vật lý
        }

        // Try to find the player object by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag(targetTag);
        if (playerObject != null)
        {
            target = playerObject.transform;
            Debug.Log($"Target found: {target.name}");
        }
        else
        {
            Debug.LogWarning($"No object with tag '{targetTag}' found!");
        }
    }

    void Update()
    {
        // If no target is assigned, try to find it again
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(targetTag);
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                return; // No target found, nothing to do
            }
            
        }

        
        // Calculate direction to target
        Vector3 direction = target.position - transform.position;
        float distanceToTarget = direction.magnitude;

        // Check if we need to move
        if (distanceToTarget > stoppingDistance)
        {
            // Normalize direction
            direction.Normalize();

            // Calculate target velocity
            Vector3 targetVelocity = direction * speed;

            // Smoothly accelerate toward target velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            // Move the object
            transform.position += currentVelocity * Time.deltaTime;

            // Rotate toward the target if enabled
            //if (lookAtTarget)
            //{
            //    Quaternion targetRotation = Quaternion.LookRotation(direction);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //}
        }
        else
        {
            // At stopping distance, slow down
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, acceleration * Time.deltaTime);
            transform.position += currentVelocity * Time.deltaTime;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Projectile hit: {other.gameObject.name} with tag: {other.tag}");
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            Debug.Log("Destroying projectile due to collision with: " + other.tag);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"Projectile collision with: {other.gameObject.name} with tag: {other.gameObject.tag}");
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Destroying projectile due to collision with: " + other.gameObject.tag);
            Destroy(gameObject);
        }
    }

    // Draw gizmos to visualize the stopping distance in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    
}
