using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{


    [Header("Horizontal Setting")]
    [SerializeField] private float speed = 2f;

    [Header("Verical Setting")]
    [SerializeField] private float jumpForce = 10f;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [SerializeField] GameObject wingEffect;
    [SerializeField] Transform wingEffectTransform;
    [Space(5)]

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask collisionMask;
    [Space(5)]

    [Header("Wall Jump")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isWallSliding;
    bool isWallJumping;
    [Space(5)]

    [Header("Dash Setting")]
    private bool canDash = true;
    private bool dashed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [SerializeField] GameObject dashEffect;
    private float gravity;
    [Space(5)]

    [Header("Attacking Setting")]
    bool attack = false;
    [SerializeField] float timeBetweenAttack;
    private float timeSinceAttack;
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    [SerializeField] GameObject downSlashEffect;
    [SerializeField] GameObject upSlashEffect;
    [Space(5)]

    [Header("Recoil Setting")]
    [SerializeField] int recoilXStep = 5;
    [SerializeField] int recoilYStep = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepXRecoilded, stepYRecoilded;
    [Space(5)]

    [Header("Health Setting")]
    public int health;
    public int maxHealth;
    public delegate void OnHealthChangeDelegate();
    [HideInInspector] public OnHealthChangeDelegate onHealthChangeCallback;
    [SerializeField] float hitFlashSpeed;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]


    [Header("Mana Setting")]
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    public bool breakMana;

    [Space(5)]

    [Header("Spell Setting")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;
    [SerializeField] GameObject sideSpellFireBall;
    [SerializeField] GameObject upSpellBloom;
    [SerializeField] GameObject downSpellFireBall;
    float TimeSinceCast;
    float castOrHealTimer;
    [Space(5)]


    [Header("Audio")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip spellCastSound;
    [SerializeField] AudioClip hurtSound;


    private SpriteRenderer sr;

    public float xAxis, yAxis;
    public Animator anim;
    [HideInInspector] public PlayerStateList pState;
    public Rigidbody2D rb;

    public static PlayerController Instance;

    bool restoreTime;
    float restoreTimeSpeed;

    //Unlocking
    public bool unlockWallJump;
    public bool unlockDash;
    public bool unlockVarJump;
    public bool unlockSideCast;
    public bool unlockUpCast;
    public bool unlockDownCast;

    public int point;

    private bool landingSoundPlayed;
    private AudioSource audioSource;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        pState = GetComponent<PlayerStateList>();

        sr = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();


        gravity = rb.gravityScale;
        Health = maxHealth;
        Mana = mana;
        manaStorage.fillAmount = Mana;

        SaveData.Instance.LoadPlayerData();

        pState.lookingRight = true;
        pState.alive = true;
    }

    
    private void Update()
    {
        
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    SaveData.Instance.SavePlayerData();
        //}
        //Debug.Log(pState.alive);

        if (GameManager.Instance.gameIsPaused) return;
        if (pState.cutScene) return;
        if (pState.alive)
        {
            GetInput();
        }

        RestoreTimeScale();
        UpdateJumpVariable();

        if (pState.dashing /* + healing*/) return;
        if (pState.alive)
        {
            Heal();
        }
        if (pState.alive)
        {
            if (!isWallJumping)
            {
                Movement();
                Jump();
                Flip();
            }
            if (unlockWallJump)
            {
                WallSlide();
                WallJump();
            }

            if (unlockDash)
            {
                StartDash();
            }
            
            Attack();          
            CastSpell();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Death());
        }


        FlashWhileInvincible();

        //WallSlide();
        //WallJump();
        //Debug.Log(Health);
    }

    private void FixedUpdate()
    {
        if (pState.cutScene) return;
        if (pState.dashing) return;
        Recoil();
    }
    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        anim.SetBool("isRunning", rb.velocity.x != 0 && IsGrounded());

    }


    private void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ?
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    private void StartDash()
    {
        
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (IsGrounded())
        {
            dashed = false;
        }
    }
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("takeDamage");

        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (onHealthChangeCallback != null)
                {
                    onHealthChangeCallback.Invoke();
                }
            }
        }
    }

    public float Mana
    {
        get
        {
            return mana;
        }
        set
        {
            if (mana != value)
            {
                if (!breakMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);

                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 1); //0.5f);
                }
                manaStorage.fillAmount = Mana;
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;

        anim.SetTrigger("Dashing");
        audioSource.PlayOneShot(dashSound);
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        /*if (IsGrounded())*/
        //Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");

        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        
    }
    private void Flip()
    {
        if (xAxis < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            pState.lookingRight = true;
        }
    }

    private void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.2f && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            pState.healing = true;
            //healing animation anim.SetBool("Healing", true);

            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }
            //Dain mana
            Mana -= Time.deltaTime * manaDrainSpeed;


        }
        else
        {
            pState.healing = false;
            healTimer = 0;
        }
    }
    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            audioSource.PlayOneShot(jumpSound);
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }
        //Double jump
        if (!IsGrounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && unlockVarJump)
        {
            audioSource.PlayOneShot(landingSound);
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            anim.SetTrigger("doubleJump");
            Instantiate(wingEffect, wingEffectTransform);

        }
        //if(!IsGrounded() && pState.jumping)
        //{
        //    anim.SetBool("isFalling", true);
        //} else
        //{
        //    anim.SetBool("isFalling", false);
        //}



        anim.SetBool("isJumping", !IsGrounded());

    }

    void UpdateJumpVariable()
    {
        if (IsGrounded())
        {
            if (!landingSoundPlayed)
            {
                audioSource.PlayOneShot(landingSound);
                landingSoundPlayed = true;
            }
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundPlayed = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    private bool IsGrounded()
    {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckY, collisionMask)
            || Physics2D.Raycast(groundCheck.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, collisionMask)
            || Physics2D.Raycast(groundCheck.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, collisionMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;


            if (yAxis == 0 || yAxis < 0 && IsGrounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

                anim.SetTrigger("Attacking");
                audioSource.PlayOneShot(attackSound);
                Instantiate(slashEffect, SideAttackTransform);
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.left * _recoilLeftOrRight, recoilXSpeed);

            }
            else if (yAxis > 0)
            {
                anim.SetTrigger("upSlash");
                Instantiate(upSlashEffect, UpAttackTransform);
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);


            }
            else if (yAxis < 0 && !IsGrounded())
            {
                anim.SetTrigger("downSlash");
                Instantiate(downSlashEffect, DownAttackTransform);
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);



            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<EnemyController>() != null)
            {
                objectsToHit[i].GetComponent<EnemyController>().EnemyHit(damage, _recoilDir, recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += manaGain;
                }
            }
        }
    }

   
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    private void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {

                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //Stop recoil
        if (pState.recoilingX && stepXRecoilded < recoilXStep)
        {
            stepXRecoilded++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepYRecoilded < recoilYStep)
        {
            stepYRecoilded++;
        }
        else
        {
            StopRecoilY();
        }
        if (IsGrounded())
        {
            StopRecoilY();
        }

    }

    private void StopRecoilX()
    {
        stepXRecoilded = 0;
        pState.recoilingX = false;
    }
    private void StopRecoilY()
    {
        stepYRecoilded = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {

        if (pState.alive)
        {
            audioSource.PlayOneShot(hurtSound);
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());

            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }

        }


    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.2f && TimeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            TimeSinceCast = 0f;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            TimeSinceCast += Time.deltaTime;
        }
        if (!Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer = 0;
        }



        //DownSpell Bug


        if (IsGrounded())
        {
            downSpellFireBall.SetActive(false);
        }

        //Down spell active -> force olayer down until grounded
        if (downSpellFireBall.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }
    IEnumerator CastCoroutine()
    {
        
        //Side Spell
        if ((yAxis == 0 || (yAxis < 0 & IsGrounded())) && unlockSideCast)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.25f /*Casting Time, need fixed*/);

            GameObject _fireBall = Instantiate(sideSpellFireBall, SideAttackTransform.position, Quaternion.identity);

            //Flip spell
            if (pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f);
        }

        //Up spell
        else if (yAxis > 0 && unlockUpCast)
        {
            audioSource.PlayOneShot(spellCastSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.25f /*Casting Time, need fixed*/);

            Instantiate(upSpellBloom, transform);
            rb.velocity = Vector2.zero;

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f);
        }

        //Down Spell
        else if ((yAxis < 0 && !IsGrounded()) && unlockDownCast)
        {
            
            Debug.Log("DownCast");
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.25f /*Casting Time, need fixed*/);

            downSpellFireBall.SetActive(true);

            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.35f);
        }

        
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.GetComponent<EnemyController>() != null && pState.casting)
        {
            _collision.GetComponent<EnemyController>().EnemyHit(spellDamage, (_collision.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        //If exit gate is upward
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //Exit direction rq horizontal move

        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Movement();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutScene = false;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        //Instantiate blood code//
        anim.SetTrigger("death");

        yield return new WaitForSeconds(0.9f);

        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

        yield return new WaitForSeconds(0.9f);
        Instantiate(GameManager.Instance.shade, transform.position, Quaternion.identity);
        Respawned();
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            breakMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.BreakMana);
            Mana = 0f;
            Health = maxHealth;
            anim.Play("idle");
        }
    }
    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.4f, wallLayer);
    }

    private void WallSlide()
    {
        if (Walled() && !IsGrounded() && xAxis != 0) //
        {
            isWallSliding = true;
            //Debug.Log(Physics2D.OverlapCircle(wallCheck.position, 0.4f, wallLayer));
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = pState.lookingRight ? 1 : -1;

            CancelInvoke(nameof(StopWallJumping));
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            dashed = false;
            airJumpCounter = 0;

            if ((pState.lookingRight && transform.eulerAngles.y == 0) || (!pState.lookingRight && transform.eulerAngles.y != 0))
            {
                pState.lookingRight = !pState.lookingRight;
                int _yRotation = pState.lookingRight ? 0 : 180;

                transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    void StopWallJumping()
    {
        isWallJumping = false;
    }
}