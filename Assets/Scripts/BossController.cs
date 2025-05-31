using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject blood;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    [HideInInspector][SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    protected Animator anim;
    

    protected enum BossStates
    {
        //Mage
        Mage_Teleport,
        Mage_Projectile1,
        Mage_Projectile2,
        Mage_Projectile3,
        Mage_Rush,
        Mage_Roar,
        Mage_Tired,
        Mage_Quake,


    }
    protected virtual BossStates GetCurrentBossState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected BossStates currentEnemyState;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerController.Instance;

    }


    protected virtual void Update()
    {

        if (GameManager.Instance.gameIsPaused) return;
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateBossState();
        }
    }


    protected virtual void UpdateBossState()
    {

    }
    protected virtual void ChangeCurrentAnimation() { }

    public virtual void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damage;
        if (!isRecoiling)
        {
            GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
            Destroy(_blood, 5.5f);
            rb.velocity = (-_hitForce * recoilFactor * _hitDirection);
        }
    }
    //Override point value each enemy; DONE point -> save point to file game. reset point to 0 when die;
    protected virtual void Death(float _destroyTime)
    {
        PlayerController.Instance.point++;
        Destroy(gameObject, _destroyTime);

    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }

        }
    }

    protected virtual void ChangeState(BossStates _newState)
    {
        GetCurrentBossState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

}