using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MossCrawler : EnemyController
{
    float timer;
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.MossCrawler_Idle);
    }

    protected override void Update()
    {
        base.Update();
        if (MossCrawlerDetect.Instance.detected && (currentEnemyState == EnemyStates.MossCrawler_Idle)) 
        {      
            ChangeState(EnemyStates.MossCrawler_Appear);
        }
        
    }

    protected override void UpdateEnemyState()
    {


        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.MossCrawler_Appear:
                anim.SetBool("detected", true);
                ChangeState(EnemyStates.MossCrawler_Run);
                break;

            case EnemyStates.MossCrawler_Run: //Wrong side when moving xD -> fix it later
                
                Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.MossCrawler_Flip);
                }
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {   
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                
                break;
            case EnemyStates.MossCrawler_Flip:
                timer += Time.deltaTime;

                if (timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.MossCrawler_Idle);
                }
                break;

        }
    }
}
