using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Tốc độ di chuyển của sóng
    [SerializeField] private float lifetime = 2f; // Thời gian tồn tại của sóng
    [SerializeField] private bool moveLeft = false; // Hướng di chuyển của sóng

    private void Start()
    {
        // Xóa sóng sau một khoảng thời gian
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Di chuyển sóng theo hướng đã chọn
        float direction = moveLeft ? -1f : 1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }
} 