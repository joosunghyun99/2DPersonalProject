using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State { Run, Slide, Jump, Hit }

    public CharacterData characterData;
    private StateMachine stateMachine;
    private State currentState;

    private Dictionary<string, BaseState> stateDic = new Dictionary<string, BaseState>();

    [Header("GroundCheck")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider;
    public Animator anim;

    [Header("Parameter")]
    public int maxHp;
    public int curHp;
    public int jumpCount;
    public float jumpPower;
    public float magnetRadius;

    public bool isGrounded;
    public bool jumpRequested;
    public bool isInvincible;

    private void Awake()
    {
        if (characterData == null) 
        {
            Debug.Log("CharacterData null");
            return;
        }

        maxHp = characterData.charHP;
        curHp = maxHp;
        jumpCount = characterData.charJumpCount;
        jumpPower = characterData.charJumpPower;
        magnetRadius = characterData.charMagnetRadius;
        isInvincible = false;

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.AddState(State.Run, new RunState(this));
        stateMachine.AddState(State.Jump, new JumpState(this));
        stateMachine.AddState(State.Slide, new SlideState(this));
        stateMachine.AddState(State.Hit, new HitState(this));
        stateMachine.InitState(State.Run);

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void LateUpdate()
    {

    }

    private void FixedUpdate()
    {
        if (jumpRequested && jumpCount > 0)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount--;
        }
        jumpRequested = false;

        if (isGrounded && rb.velocity.y <= 0.05f)
        {
            jumpCount = characterData.charJumpCount;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    IEnumerator BlinkCo(float duration)
    {
        float timer = 0.0f;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), true);

        while (timer < duration)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.2f);

            timer += 0.4f;
        }

        isInvincible = false;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), false);
    }

    private class RunState : BaseState
    {
        public RunState(Character owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Run");
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                owner.jumpCount--;
                ChangeState(State.Jump);
            }

            if (Input.GetKeyDown(KeyCode.S)) 
            {
                ChangeState(State.Slide);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
            {
                ChangeState(State.Hit);
            }
        }
    }

    private class JumpState : BaseState
    {
        public JumpState(Character owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Jump");
            owner.rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                owner.jumpCount--;
                ChangeState(State.Jump);
            }

            if (isGrounded && rb.velocity.y <= 0.05f)
            {
                ChangeState(State.Run);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
            {
                ChangeState(State.Hit);
            }
        }
    }

    private class SlideState : BaseState
    {
        public SlideState(Character owner) : base(owner) { }
        public override void Enter()
        {
            Debug.Log("Slide");
            owner.transform.localScale = new Vector3(2.0f, 0.5f, 1.0f);
            owner.capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                owner.jumpCount--;
                ChangeState(State.Jump);
            }
            else if (Input.GetKeyUp(KeyCode.S)) 
            {
                ChangeState(State.Run);
            }
        }

        public override void Exit()
        {
            owner.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            owner.capsuleCollider.direction = CapsuleDirection2D.Vertical;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
            {
                ChangeState(State.Hit);
            }
        }
    }

    private class HitState : BaseState
    {
        public HitState(Character owner) : base(owner) { }

        public void Enter() 
        {
            owner.curHp--;
            owner.isInvincible = true;
            owner.StartCoroutine(owner.BlinkCo(2.0f));
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                owner.jumpCount--;
                ChangeState(State.Jump);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeState(State.Slide);
            }
            else if (isInvincible == false) 
            {
                ChangeState(State.Run);
            }
        }
    }
}
