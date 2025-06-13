using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float moveSpeed = 10f;         // Tốc độ di chuyển của sóng
    [SerializeField] private float damage = 10f;            // Sát thương gây ra
    [SerializeField] private float knockbackForce = 5f;     // Lực đẩy lùi
    [SerializeField] private float destroyDistance = 20f;   // Khoảng cách tối đa trước khi tự hủy

    [Header("Visual Settings")]
    [SerializeField] private Color waveColor = Color.cyan;  // Màu cố định của sóng
    private bool isLeftWave = false;                       // Xác định hướng sóng
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;                          // Vị trí bắt đầu
    private Vector3 moveDirection;                          // Hướng di chuyển

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Đảm bảo có Collider2D
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Set là trigger để không có va chạm vật lý
        }

        // Thiết lập hướng di chuyển và quay hình ảnh
        moveDirection = isLeftWave ? Vector3.left : Vector3.right;
        if (isLeftWave)
        {
            // Quay ngược hình ảnh khi đi về bên trái
            spriteRenderer.flipX = true;
        }

        // Thiết lập màu cố định
        spriteRenderer.color = waveColor;

        // Lưu vị trí bắt đầu
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển theo hướng đã định
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Kiểm tra khoảng cách đã di chuyển
        float distanceTraveled = Vector3.Distance(transform.position, startPosition);
        if (distanceTraveled >= destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Tính toán hướng đẩy lùi dựa trên hướng sóng
                Vector2 knockbackDirection = isLeftWave ? Vector2.left : Vector2.right;
                
                // Gây sát thương và đẩy lùi
                player.TakeDamage(damage);
                player.GetComponent<Rigidbody2D>()?.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
            // Tự hủy sau khi va chạm với người chơi
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Tính toán hướng đẩy lùi dựa trên hướng sóng
                Vector2 knockbackDirection = isLeftWave ? Vector2.left : Vector2.right;
                
                // Gây sát thương và đẩy lùi
                player.TakeDamage(damage);
                player.GetComponent<Rigidbody2D>()?.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
            // Tự hủy sau khi va chạm với người chơi
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        // Vẽ gizmo để dễ nhìn trong Scene view
        Gizmos.color = waveColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    // Phương thức để thiết lập hướng sóng từ bên ngoài
    public void SetDirection(bool isLeft)
    {
        isLeftWave = isLeft;
    }
}
