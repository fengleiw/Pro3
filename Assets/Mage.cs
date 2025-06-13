using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : BossController
{
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;
    [SerializeField] private float minY = -1.5f;
    [SerializeField] private float maxY = 2f;


    [SerializeField] Transform pillarPos;
    [SerializeField] private GameObject pillarPre;
    [Header("Projectile Setting")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform position;
    [SerializeField] Transform rushPositionLeft;
    [SerializeField] Transform rushPositionRight;
    [SerializeField] private float projectileDelay = 3f; // Thời gian delay giữa các projectile
    [SerializeField] private int projectileCount = 3; // Số lượng projectile mỗi lần tấn công
    private int currentProjectileCount = 0;
    private float nextProjectileTime = 0f;

    [Header("Effect Settings")]
    [SerializeField] private GameObject shockwaveEffect; // Hiệu ứng sóng xung kích khi quake
    [SerializeField] private float shockwaveOffset = 1f; // Khoảng cách giữa boss và sóng xung kích

    [Header("Phase Settings")]
    [SerializeField] private float phase2HealthThreshold = 50f;
    [SerializeField] private float attackCooldown = 3f; // Tăng thời gian cooldown
    [SerializeField] private float phase2SpeedMultiplier = 1.5f;

    [Header("Rush Settings")]
    [SerializeField] private float rushSpeed = 15f;
    [SerializeField] private float groundY = -1.5f;
    [SerializeField] private bool isRushing = false;
    private SpriteRenderer spriteRenderer; // Thêm biến để lưu SpriteRenderer

    [Header("Teleport Settings")]
    [SerializeField] private int maxTeleportCount = 3;
    [SerializeField] private float teleportDelay = 0.8f; // Delay giữa các lần teleport
    [SerializeField] private float minTeleportDistance = 3f; // Khoảng cách tối thiểu giữa các lần teleport
    private int currentTeleportCount = 0;
    private float nextTeleportTime = 0f;
    private bool isTeleporting = false;
    private Vector2 lastTeleportPosition;

    [Header("Quake Settings")]
    [SerializeField] private float quakePreparationTime = 1f; // Thời gian chuẩn bị trước khi lao xuống
    [SerializeField] private float quakeRecoveryTime = 1.5f; // Thời gian nghỉ sau khi quake
    private bool isQuakePreparation = false;
    private float quakeStateStartTime = 0f;

    private float nextAttackTime;
    private bool isPhase2 = false;
    private BossStates lastUsedAttack = BossStates.Mage_Teleport;
    private bool attackInProgress = false;

    // Attack pattern arrays
    private BossStates[] phase1Attacks = {
        BossStates.Mage_Teleport,
        BossStates.Mage_Projectile1,
        BossStates.Mage_Rush,
        BossStates.Mage_Quake,
        BossStates.Mage_Roar
    };

    private BossStates[] phase2Attacks = {
        BossStates.Mage_Teleport,
        BossStates.Mage_Projectile1,
        BossStates.Mage_Projectile2,
        BossStates.Mage_Rush,
        BossStates.Mage_Quake,
        BossStates.Mage_Roar
    };

    [SerializeField] GameObject Projectile2;
    [SerializeField] Transform pro2Pos;

    protected override void Start()
    {
        base.Start();
        nextAttackTime = Time.time + attackCooldown;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy component SpriteRenderer

        // Start with random attack
        ChangeToRandomAttack();
    }

    protected override void Update()
    {
        UpdateBossState();
    }

    protected override void UpdateBossState()
    {
        if (health <= 0)
        {
            Death(0.05f);
            return;
        }

        // Check phase transition
        CheckPhaseTransition();

        // Handle attack timing and state changes
        HandleAttackTiming();

        // Execute current state
        ExecuteCurrentState();
    }

    private void CheckPhaseTransition()
    {
        if (!isPhase2 && health <= phase2HealthThreshold)
        {
            isPhase2 = true;
            // Giảm cooldown nhẹ hơn cho phase 2
            attackCooldown = Mathf.Max(attackCooldown / 1.3f, 1.5f);

            Debug.Log("Phase 2 activated!");
        }
    }

    private void HandleAttackTiming()
    {
        // Chỉ đổi chiêu khi không có attack đang diễn ra và đã hết cooldown
        if (Time.time >= nextAttackTime && !attackInProgress)
        {
            ChangeToRandomAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void ChangeToRandomAttack()
    {
        BossStates[] availableAttacks = isPhase2 ? phase2Attacks : phase1Attacks;
        BossStates newAttack;

        // Tránh lặp lại chiêu thức giống nhau
        do
        {
            newAttack = availableAttacks[Random.Range(0, availableAttacks.Length)];
        }
        while (newAttack == lastUsedAttack && availableAttacks.Length > 1);

        lastUsedAttack = newAttack;
        ChangeState(newAttack);

        // Reset các biến trạng thái
        ResetAttackStates();
    }

    private void ResetAttackStates()
    {
        attackInProgress = false;
        currentTeleportCount = 0;
        isTeleporting = false;
        nextTeleportTime = 0f;
        isQuakePreparation = false;
        quakeStateStartTime = 0f;
        lastTeleportPosition = Vector2.zero;
        currentProjectileCount = 0;
        nextProjectileTime = 0f;
    }

    private void ExecuteCurrentState()
    {
        float currentSpeedMultiplier = isPhase2 ? phase2SpeedMultiplier : 1f;

        switch (GetCurrentBossState)
        {
            case BossStates.Mage_Teleport:
                ExecuteTeleport();
                break;

            case BossStates.Mage_Projectile1:
                ExecuteProjectile();
                break;

            case BossStates.Mage_Projectile2:
                ExecuteProjectile();
                break;

            case BossStates.Mage_Projectile3:
                ExecuteProjectile();
                break;

            case BossStates.Mage_Roar:
                ExecuteRoar();
                break;

            case BossStates.Mage_Rush:
                ExecuteRush(currentSpeedMultiplier);
                break;

            case BossStates.Mage_Quake:
                ExecuteQuake(currentSpeedMultiplier);
                break;
        }
    }

    private void ExecuteTeleport()
    {
        if (!isTeleporting)
        {
            isTeleporting = true;
            attackInProgress = true;
            nextTeleportTime = Time.time + teleportDelay;
            lastTeleportPosition = transform.position; // Lưu vị trí hiện tại
            anim.SetBool("Teleport", true);
        }

        // Kiểm tra thời gian để teleport tiếp theo
        if (Time.time >= nextTeleportTime && currentTeleportCount < maxTeleportCount)
        {
            Vector2 newPos = GetValidTeleportPosition();
            transform.position = newPos;
            lastTeleportPosition = newPos; // Cập nhật vị trí teleport cuối

            currentTeleportCount++;
            nextTeleportTime = Time.time + teleportDelay;

            Debug.Log($"Teleport {currentTeleportCount}/{maxTeleportCount} to position: {newPos}");

            // Add teleport effect here if needed
            // Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }

        // Kết thúc teleport sau khi đã teleport đủ số lần
        if (currentTeleportCount >= maxTeleportCount)
        {
            attackInProgress = false;
            isTeleporting = false;
            anim.SetBool("Teleport", false);
        }
    }

    private Vector2 GetValidTeleportPosition()
    {
        Vector2 newPos;
        int attempts = 0;
        int maxAttempts = 20; // Tránh vòng lặp vô hạn

        do
        {
            float posX = Random.Range(minX, maxX);
            float posY = Random.Range(minY, maxY);
            newPos = new Vector2(posX, posY);
            attempts++;

            // Nếu thử quá nhiều lần mà không tìm được vị trí phù hợp, chấp nhận vị trí hiện tại
            if (attempts >= maxAttempts)
            {
                break;
            }
        }
        while (Vector2.Distance(newPos, lastTeleportPosition) < minTeleportDistance);

        return newPos;
    }

    private void ExecuteProjectile()
    {
        if (!attackInProgress)
        {
            attackInProgress = true;
            currentProjectileCount = 0;
            nextProjectileTime = Time.time;
        }

        // Kiểm tra vị trí Y và di chuyển lên nếu cần
        if (transform.position.y < -1.17f)
        {
            // Di chuyển lên vị trí cao hơn
            float targetY = Random.Range(-1.17f, maxY);
            transform.position = new Vector2(transform.position.x, targetY);
            Debug.Log("Mage moving up to shoot projectiles");
            return; // Chờ frame tiếp theo để kiểm tra lại
        }

        // Kiểm tra và tạo projectile mới nếu đã đến thời gian và ở vị trí phù hợp
        if (attackInProgress && Time.time >= nextProjectileTime && currentProjectileCount < projectileCount && transform.position.y >= -1.17f)
        {
            if (projectile != null && position != null)
            {
                Instantiate(projectile, position.position, position.rotation);
                currentProjectileCount++;
                nextProjectileTime = Time.time + projectileDelay;
                anim.SetTrigger("Cast");
                Debug.Log($"Tạo projectile {currentProjectileCount}/{projectileCount}");
            }

            // Kết thúc đòn tấn công khi đã bắn đủ số projectile
            if (currentProjectileCount >= projectileCount)
            {
                StartCoroutine(EndProjectileAttack());
            }
        }
    }

    private IEnumerator EndProjectileAttack()
    {
        yield return new WaitForSeconds(0.5f); // Chờ 0.5s trước khi có thể dùng chiêu khác
        attackInProgress = false;
        currentProjectileCount = 0;
    }

    private void ExecuteRoar()
    {
        if (!attackInProgress)
        {
            attackInProgress = true;
            Debug.Log("Boss is roaring!");

            // Spawn Projectile2 and RotatingObject only once
            if (Projectile2 != null && pro2Pos != null)
            {
                Instantiate(Projectile2, pro2Pos.position, pro2Pos.rotation);
            }

            StartCoroutine(EndRoarAttack());
        }
    }

    private IEnumerator EndRoarAttack()
    {
        yield return new WaitForSeconds(1.5f); // Roar kéo dài 1.5s
        attackInProgress = false;
    }

    private void ExecuteRush(float speedMultiplier)
    {
        if (!isRushing && !attackInProgress)
        {
            attackInProgress = true;

            // Randomly choose direction: 0 = left to right, 1 = right to left
            int rushDirection = Random.Range(0, 2);

            if (rushDirection == 0)
            {
                // Rush from left to right
                transform.position = new Vector2(rushPositionLeft.position.x, rushPositionLeft.position.y);
                rb.velocity = new Vector2(rushSpeed * speedMultiplier, 0);
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false; // Không lật khi đi sang phải
                }
                anim.SetTrigger("rush");
            }
            else
            {
                // Rush from right to left
                transform.position = new Vector2(rushPositionRight.position.x, rushPositionRight.position.y);
                rb.velocity = new Vector2(-rushSpeed * speedMultiplier, 0);
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = true; // Lật khi đi sang trái
                }
                anim.SetTrigger("rush");
            }

            isRushing = true;
            Debug.Log("Rush started!");
        }

        // Kiểm tra kết thúc rush - CHỈ KIỂM TRA KHI ĐANG RUSH
        if (isRushing && attackInProgress)
        {
            if ((rb.velocity.x > 0 && transform.position.x >= maxX) ||
                (rb.velocity.x < 0 && transform.position.x <= minX))
            {
                rb.velocity = Vector2.zero;
                isRushing = false;
                attackInProgress = false; // Kết thúc attack, tự động chuyển chiêu khác
                Debug.Log("Rush completed - changing to next attack!");

                // Force change to next attack immediately
                nextAttackTime = Time.time; // Reset cooldown để ngay lập tức chuyển chiêu
            }
        }
    }

    private void ExecuteQuake(float speedMultiplier)
    {
        if (!attackInProgress && !isRushing && !isQuakePreparation)
        {
            // Bắt đầu quake - giai đoạn chuẩn bị
            attackInProgress = true;
            isQuakePreparation = true;
            quakeStateStartTime = Time.time;

            // Teleport to random position above ground
            float telePosX = Random.Range(minX, maxX);
            float telePosY = Random.Range(maxY - 1f, maxY);
            Vector2 teleportPosition = new Vector2(telePosX, telePosY);
            transform.position = teleportPosition;
            anim.SetTrigger("QuakePrepare");
            Debug.Log("Quake preparation - Boss positioning above ground");

            // Add preparation effect here (glowing, charging animation, etc.)
            // Instantiate(quakeChargeEffect, transform.position, Quaternion.identity);
        }

        // Giai đoạn chuẩn bị - chờ trước khi lao xuống
        if (isQuakePreparation && !isRushing)
        {
            
            if (Time.time - quakeStateStartTime >= quakePreparationTime)
            {
                // Bắt đầu lao xuống
                isQuakePreparation = false;
                isRushing = true;
                rb.velocity = new Vector2(0, -rushSpeed * speedMultiplier);
                Instantiate(pillarPre, pillarPos);
                Debug.Log("Quake attack - Boss rushing down!");
            }
        } 

        // Giai đoạn lao xuống và va chạm
        if (isRushing && !isQuakePreparation && transform.position.y <= groundY)
        {
            // Stop at ground level
            transform.position = new Vector2(transform.position.x, groundY);
            rb.velocity = Vector2.zero;
            isRushing = false;

            // Tạo hiệu ứng sóng xung kích ở hai bên
            if (shockwaveEffect != null)
            {
                // Tạo sóng bên phải (di chuyển sang phải)
                Vector2 rightPos = new Vector2(transform.position.x + shockwaveOffset, transform.position.y);
                GameObject rightWave = Instantiate(shockwaveEffect, rightPos, Quaternion.identity);
                ShockWave rightWaveScript = rightWave.GetComponent<ShockWave>();
                if (rightWaveScript != null)
                {
                    rightWaveScript.SetDirection(false); // false = di chuyển sang phải
                }
                
                // Tạo sóng bên trái (di chuyển sang trái)
                Vector2 leftPos = new Vector2(transform.position.x - shockwaveOffset, transform.position.y);
                GameObject leftWave = Instantiate(shockwaveEffect, leftPos, Quaternion.identity);
                ShockWave leftWaveScript = leftWave.GetComponent<ShockWave>();
                if (leftWaveScript != null)
                {
                    leftWaveScript.SetDirection(true); // true = di chuyển sang trái
                }
            }

            // Bắt đầu giai đoạn recovery
            quakeStateStartTime = Time.time;

            Debug.Log("Quake impact - Ground shaking with shockwaves!");

            // Add screen shake or ground impact effects here
            // ScreenShake.Instance.Shake(0.5f, 0.3f);
        }

        // Giai đoạn recovery - nghỉ sau khi quake
        if (!isRushing && !isQuakePreparation && attackInProgress)
        {
            if (Time.time - quakeStateStartTime >= quakeRecoveryTime)
            {
                attackInProgress = false; // Kết thúc hoàn toàn quake attack
                Debug.Log("Quake completed - Ready for next attack");
            }
        }
    }

    // Public method to manually trigger phase 2 (for testing)
    public void ForcePhase2()
    {
        health = phase2HealthThreshold - 1;
        CheckPhaseTransition();
    }

    // Getter for current phase (useful for UI or other systems)
    public bool IsPhase2()
    {
        return isPhase2;
    }
}