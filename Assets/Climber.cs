using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : EnemyController
{
    float timer;
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    public static Climber Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }


    protected override void Update()
    {
        base.Update();
        
        ChangeState(EnemyStates.Climber_Idle);
        
    }
    protected override void UpdateEnemyState()
    {


        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Climber_Idle:
                Debug.Log("IDLE");
                Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;
            case EnemyStates.Climber_Flip:
                Debug.Log("1");
                break;

        }
    }
}
