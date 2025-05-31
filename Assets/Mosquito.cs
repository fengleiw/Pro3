using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dasher : EnemyController
{
    [Header("Dasher Settings")]
    [SerializeField] private float detectDistance = 8f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.8f;
    [SerializeField] private float cooldownDuration = 2f;
    [SerializeField] private float stunDuration = 1.5f;

    private float timer;
    private Vector2 dashDirection;
    private bool isDashing = false;

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Dasher_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Dasher_Idle);
        }
    }

    protected override void UpdateEnemyState()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Dasher_Idle:
                rb.velocity = Vector2.zero;

                if (_dist < detectDistance)
                {
                    ChangeState(EnemyStates.Dasher_Charging);
                }
                break;

            case EnemyStates.Dasher_Charging:
                
                rb.velocity = Vector2.zero;
                FlipDasher();

                timer += Time.deltaTime;
                if (timer >= 0.5f) 
                {
                    StartDash();
                    timer = 0;
                }
                break;

            case EnemyStates.Dasher_Dashing:
                
                if (isDashing)
                {
                    rb.velocity = dashDirection * dashSpeed;

                    timer += Time.deltaTime;
                    if (timer >= dashDuration)
                    {
                        ChangeState(EnemyStates.Dasher_Cooldown);
                        timer = 0;
                        isDashing = false;
                    }
                }
                break;

            case EnemyStates.Dasher_Cooldown:
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 5f);

                timer += Time.deltaTime;
                if (timer >= cooldownDuration)
                {
                    ChangeState(EnemyStates.Dasher_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Dasher_Stunned:
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime * 3f);

                timer += Time.deltaTime;
                if (timer >= stunDuration)
                {
                    ChangeState(EnemyStates.Dasher_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Dasher_Death:
                Death(Random.Range(1, 3));
                break;
        }
    }

    void StartDash()
    {
        
        Vector2 playerPos = PlayerController.Instance.transform.position;
        dashDirection = (playerPos - (Vector2)transform.position).normalized;

        ChangeState(EnemyStates.Dasher_Dashing);
        isDashing = true;
    }

    void FlipDasher()
    {
        sr.flipX = PlayerController.Instance.transform.position.x > transform.position.x;
    }

    public override void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damage, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Dasher_Stunned);
            timer = 0;
        }
        else
        {
            ChangeState(EnemyStates.Dasher_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        rb.velocity = Vector2.zero;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("idle", GetCurrentEnemyState == EnemyStates.Dasher_Idle);
        anim.SetBool("charging", GetCurrentEnemyState == EnemyStates.Dasher_Charging);
        anim.SetBool("dashing", GetCurrentEnemyState == EnemyStates.Dasher_Dashing);
        //anim.SetBool("cooldown", GetCurrentEnemyState == EnemyStates.Dasher_Cooldown);
        anim.SetBool("stunned", GetCurrentEnemyState == EnemyStates.Dasher_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Dasher_Death)
        {
            anim.SetTrigger("death");
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectDistance);

        if (isDashing && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, dashDirection * 3f);
        }
    }
}