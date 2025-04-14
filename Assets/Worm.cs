using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Worm : EnemyController
{
    [SerializeField] GameObject DetectedField;
    [SerializeField] GameObject worm;
    float timer;
    public bool detected;
    public float wormSpeed = 10f;
    public static Worm Instance;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Worm_Appear);
    }
    protected override void Update()
    {
        base.Update();
        Debug.Log(SpawnMoss.Instance.right);
    }

    protected override void UpdateEnemyState()
    {
        if (health <= 0)
        {
            Death(0.5f);
            Destroy(SpawnMoss.Instance.gameObject);
            Destroy(gameObject);
        }
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Worm_Appear:

                ChangeState(EnemyStates.Worm_Charger);
                break;

            case EnemyStates.Worm_Charger:
                if (SpawnMoss.Instance.right)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    rb.velocity = new Vector2(-wormSpeed, 0);
                    //SpawnMoss.Instance.right = !SpawnMoss.Instance.right;
                    if (MossWalCheck.Instance.walled || !MossGroundCheck.Instance.grounded)
                    {
                        anim.SetTrigger("Disappear");
                        StartCoroutine(DestroyAfterAnimation());
                        
                    }
                }

                else
                {
                    rb.velocity = new Vector2(wormSpeed, 0);
                    //SpawnMoss.Instance.right = !SpawnMoss.Instance.right;
                    if (MossWalCheck.Instance.walled || !MossGroundCheck.Instance.grounded)
                    {
                        anim.SetTrigger("Disappear");
                       StartCoroutine(DestroyAfterAnimation());
                        
                    }
                }

                break;

        }

    }
    private IEnumerator DestroyAfterAnimation()
    {
        // Wait for the length of the "Disappear" animation
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }

}

