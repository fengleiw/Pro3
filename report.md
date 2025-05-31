#### 3.1.2. Enemy System
Hệ thống kẻ địch được thiết kế với cấu trúc phân cấp và mở rộng, sử dụng mẫu thiết kế Template Method và State Pattern để quản lý hành vi của các loại kẻ địch khác nhau.

##### a) Cấu trúc cơ bản
```csharp
public class EnemyController : MonoBehaviour
{
    protected enum EnemyStates
    {
        // Crawler states
        Crawler_Idle,
        Crawler_Flip,
        Crawler_Run,

        // Buzzer states
        Buzzer_Idle,
        Buzzer_Chase,
        Buzzer_Stunned,
        Buzzer_Death,

        // ZombieBasic states
        ZB_Idle,
        ZB_Suprised,
        ZB_Charge,

        // Shade states
        Shade_Idle,
        Shade_Chase,
        Shade_Stunned,
        Shade_Death,

        // Worm states
        Worm_Appear,
        Worm_Charger,

        // MossCrawler states
        MossCrawler_Idle,
        MossCrawler_Appear,
        MossCrawler_Flip,
        MossCrawler_Run,

        // Climber states
        Climber_Idle,
        Climber_Flip,

        // Dasher states
        Dasher_Idle,
        Dasher_Charging,
        Dasher_Dashing,
        Dasher_Cooldown,
        Dasher_Stunned,
        Dasher_Death
    }
}
```

##### b) Các loại kẻ địch

1. **ZombieBasic**
   - Hành vi cơ bản: Di chuyển qua lại trên mặt đất
   - Đặc điểm:
     - Tự động đổi hướng khi gặp chướng ngại
     - Phát hiện người chơi và chuyển sang trạng thái tấn công
     - Có khả năng nhảy và lao nhanh về phía người chơi
   - Trạng thái: Idle → Surprised → Charge

2. **Crawler**
   - Hành vi: Bò trên tường và trần
   - Đặc điểm:
     - Di chuyển theo bề mặt
     - Tự động đổi hướng khi gặp chướng ngại
     - Có khả năng bám tường
   - Trạng thái: Idle → Flip → Run

3. **Buzzer**
   - Hành vi: Bay và đuổi theo người chơi
   - Đặc điểm:
     - Bay tự do trong không gian
     - Đuổi theo người chơi khi trong tầm
     - Bị choáng khi bị tấn công
   - Trạng thái: Idle → Chase → Stunned → Death

4. **MossCrawler**
   - Hành vi: Ẩn nấp và tấn công bất ngờ
   - Đặc điểm:
     - Ẩn mình trong môi trường
     - Xuất hiện đột ngột để tấn công
     - Di chuyển nhanh khi phát hiện người chơi
   - Trạng thái: Idle → Appear → Flip → Run

5. **Climber**
   - Hành vi: Leo trèo trên tường
   - Đặc điểm:
     - Di chuyển theo bề mặt dọc
     - Có khả năng bám tường
     - Tự động đổi hướng
   - Trạng thái: Idle → Flip

6. **Dasher**
   - Hành vi: Tấn công nhanh và mạnh
   - Đặc điểm:
     - Có khả năng lao nhanh về phía người chơi
     - Cần thời gian hồi chiêu
     - Bị choáng khi bị tấn công
   - Trạng thái: Idle → Charging → Dashing → Cooldown → Stunned → Death

7. **Worm**
   - Hành vi: Xuất hiện từ dưới đất
   - Đặc điểm:
     - Ẩn mình dưới mặt đất
     - Xuất hiện đột ngột để tấn công
     - Có khả năng lao nhanh
   - Trạng thái: Appear → Charger

8. **Shade**
   - Hành vi: Kẻ địch đặc biệt
   - Đặc điểm:
     - Có khả năng đuổi theo người chơi
     - Có thể là boss hoặc kẻ địch đặc biệt
     - Có hệ thống trạng thái phức tạp
   - Trạng thái: Idle → Chase → Stunned → Death

##### c) Hệ thống Boss
```csharp
public class BossController : MonoBehaviour
{
    protected enum BossStates
    {
        // Mage Boss states
        Mage_Teleport,    // Dịch chuyển tức thời
        Mage_Projectile1, // Đạn phép loại 1
        Mage_Projectile2, // Đạn phép loại 2
        Mage_Projectile3, // Đạn phép loại 3
        Mage_Rush,        // Lao nhanh
        Mage_Roar,        // Tiếng gầm
        Mage_Tired,       // Trạng thái mệt mỏi
        Mage_Quake        // Rung chuyển
    }

    // Các thuộc tính cơ bản
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject blood;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected PlayerController player;
    [SerializeField] protected float speed;
    protected Animator anim;

    // Các phương thức cơ bản
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void UpdateBossState() { }
    protected virtual void ChangeCurrentAnimation() { }
    public virtual void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce) { }
    protected virtual void Death(float _destroyTime) { }
    protected virtual void Attack() { }
    protected virtual void ChangeState(BossStates _newState) { }
}
```

**Mage Boss**
- Hành vi: Boss pháp sư với nhiều kỹ năng
- Đặc điểm:
  - Có khả năng dịch chuyển tức thời
  - Sử dụng 3 loại đạn phép khác nhau
  - Có khả năng lao nhanh
  - Có tiếng gầm gây ảnh hưởng
  - Có trạng thái mệt mỏi
  - Có khả năng gây rung chuyển
  - Có hệ thống phase (giai đoạn) với các đòn tấn công khác nhau
  - Có hệ thống cooldown và timing cho các đòn tấn công

2.2.4 Kẻ địch
2.2.4.1 Lớp EnemyController
Điều khiển kẻ địch bằng Component: EnemyController.cs

Hình 2.7: Lớp EnemyController

Trong đó:
- Attack(): Gây sát thương lên người chơi khi va chạm bằng cách gọi PlayerController.Instance.TakeDamage(damage) để trừ máu người chơi, giá trị damage được tuỳ chỉnh theo từng loại quái.
- ChangeCurrentAnimation(): Thay đổi animation của quái vật dựa trên trạng thái hiện tại, phương thức này được ghi đè lên các lớp con để phù hợp theo từng loại quái. Sử dụng anim.SetTrigger() hoặc anim.SetBool() chuyển đổi animation.
- ChangeState(EnemyStates _newState): Thay đổi trạng thái của quái vật và kích hoạt animation tương ứng. 
- Death(float _destroyTime): Xử lý cách thức chết của quái vật, xoá đối tượng sau thời gian _destroyTime
- EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce): Xử lý khi quái vật bị tấn công. Các tham số: _damage: sát thương nhận vào; _hitDirection: Hướng bị đẩy lùi; _hitForce: Lực đẩy lùi.
- UpdateEnemyState(): Cập nhật logic hành vi của quái vật, hoạt động theo phương thức được ghi đè trong lớp con.

2.2.4.2 Lớp BossController
BossController là một class riêng biệt kế thừa từ MonoBehaviour, được thiết kế để quản lý các boss trong game:

```csharp
public class BossController : MonoBehaviour
{
    protected enum BossStates
    {
        // Mage Boss states
        Mage_Teleport,    // Dịch chuyển tức thời
        Mage_Projectile1, // Đạn phép loại 1
        Mage_Projectile2, // Đạn phép loại 2
        Mage_Projectile3, // Đạn phép loại 3
        Mage_Rush,        // Lao nhanh
        Mage_Roar,        // Tiếng gầm
        Mage_Tired,       // Trạng thái mệt mỏi
        Mage_Quake        // Rung chuyển
    }
}
```

Trong đó:
- Attack(): Gây sát thương lên người chơi khi va chạm bằng cách gọi PlayerController.Instance.TakeDamage(damage) để trừ máu người chơi, giá trị damage được tuỳ chỉnh theo từng loại boss.
- ChangeCurrentAnimation(): Thay đổi animation của boss dựa trên trạng thái hiện tại, phương thức này được ghi đè trong lớp con (như Mage) để phù hợp với từng loại boss. Sử dụng anim.SetTrigger() hoặc anim.SetBool() để chuyển đổi animation.
- ChangeState(BossStates _newState): Thay đổi trạng thái của boss và kích hoạt animation tương ứng. Các trạng thái được định nghĩa trong enum BossStates.
- Death(float _destroyTime): Xử lý cách thức chết của boss, bao gồm việc tăng điểm cho người chơi và xoá đối tượng sau thời gian _destroyTime.
- EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce): Xử lý khi boss bị tấn công. Các tham số: _damage: sát thương nhận vào; _hitDirection: Hướng bị đẩy lùi; _hitForce: Lực đẩy lùi. Có hiệu ứng máu và đẩy lùi khi bị tấn công.
- UpdateBossState(): Cập nhật logic hành vi của boss, được ghi đè trong lớp con để thực hiện các hành vi đặc biệt như teleport, projectile, rush, roar, quake.

Mage Boss là một implementation cụ thể của BossController, có thêm các tính năng:
- Hệ thống phase (giai đoạn) với các đòn tấn công khác nhau:
  + Phase 1: Teleport, Projectile1, Rush, Quake
  + Phase 2: Thêm Projectile2, Projectile3, Roar
- Hệ thống cooldown và timing cho các đòn tấn công
- Các kỹ năng đặc biệt:
  + Teleport: Dịch chuyển tức thời đến vị trí ngẫu nhiên
  + Projectile: Bắn 3 loại đạn phép khác nhau
  + Rush: Lao nhanh từ trái sang phải hoặc ngược lại
  + Roar: Gây ảnh hưởng đến người chơi
  + Quake: Lao xuống gây rung chuyển

2.3 Spike (Gai)
Spike là một loại chướng ngại vật nguy hiểm trong game, gây sát thương ngay lập tức khi người chơi chạm vào.

```csharp
public class spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        // Xử lý khi người chơi chạm vào gai
        PlayerController.Instance.pState.cutScene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        
        // Hiệu ứng màn hình
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);
        
        // Hồi sinh người chơi
        yield return new WaitForSecondsRealtime(1);
        PlayerController.Instance.transform.position = GameManager.Instance.platformingReSpawnPoint;
        
        // Kết thúc hiệu ứng
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);
        PlayerController.Instance.pState.cutScene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;
    }
}
```

2.4 Respawn Point (Điểm hồi sinh)
Respawn Point là vị trí mà người chơi sẽ được hồi sinh sau khi chết hoặc rơi vào gai. Hệ thống này được quản lý bởi GameManager:

```csharp
public class GameManager : MonoBehaviour
{
    public Vector2 platformingReSpawnPoint; // Vị trí hồi sinh
    public GameObject shade; // Hiệu ứng khi chết
    
    // Các phương thức quản lý điểm hồi sinh
    public void SetRespawnPoint(Vector2 newPoint)
    {
        platformingReSpawnPoint = newPoint;
    }
}
```

2.5 Save Point (Điểm lưu)
Save Point cho phép người chơi lưu tiến trình game. Hệ thống này sử dụng cấu trúc SaveData để lưu trữ thông tin:

```csharp
[System.Serializable]
public struct SaveData
{
    public static SaveData Instance;

    // Thông tin điểm lưu
    public string benchSceneName; // Tên scene chứa điểm lưu
    public Vector2 benchPos;      // Vị trí điểm lưu

    // Thông tin người chơi
    public int playerHealth;      // Máu
    public float playerMana;      // Mana
    public bool playerBreakMana;  // Trạng thái mana
    public Vector2 playerPosition; // Vị trí
    public string lastScene;      // Scene cuối cùng

    // Kỹ năng đã mở khóa
    public bool playerUnlockWallJump;
    public bool playerUnlockDash;
    public bool playerUnlockVarJump;
    public bool playerUnlockSideCast;
    public bool playerUnlockUpCast;
    public bool playerUnlockDownCast;

    // Thông tin Shade
    public Vector2 shadePos;
    public string sceneWithShade;
    public Quaternion shadeRot;

    // Phương thức lưu dữ liệu
    public void SaveBench()
    {
        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(Application.persistentDataPath + "/save.bench.data")))
        {
            writer.Write(benchSceneName);
            writer.Write(benchPos.x);
            writer.Write(benchPos.y);
        }
    }
}
```

Hệ thống lưu game sử dụng BinaryWriter để lưu dữ liệu vào file, đảm bảo tính bảo mật và hiệu suất. Dữ liệu được lưu bao gồm:
- Vị trí và scene của điểm lưu
- Trạng thái người chơi (máu, mana, vị trí)
- Các kỹ năng đã mở khóa
- Thông tin về Shade (kẻ địch đặc biệt xuất hiện khi chết) 