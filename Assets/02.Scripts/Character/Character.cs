using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData;

    private Dictionary<string, BaseState> stateDic = new Dictionary<string, BaseState>();

    private BaseState currentState;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private Animator anim;

    [Header("Parameter")]
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private int jumpCount;
    [SerializeField] private float jumpPower;
    [SerializeField] private float magnetRadius;

    private bool isGrounded;
    private bool jumpRequested;

    private void Awake()
    {
        AddState("Run", new RunState());
        AddState("Jump", new JumpState());
        AddState("Slide", new SlideState());
        AddState("Hit", new HitState());

        maxHp = characterData.charHP;
        curHp = maxHp;
        jumpCount = characterData.charJumpCount;
        jumpPower = characterData.charJumpPower;
        magnetRadius = characterData.charMagnetRadius;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        
        currentState.Update();
        
        currentState.Transition();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    jumpRequested = true;
        //}

        //if (Input.GetKeyDown(KeyCode.S)) 
        //{
        //    Slide();
        //}

        //if (Input.GetKeyUp(KeyCode.S)) 
        //{
        //    ExitSlide();
        //}
    }
    private void LateUpdate()
    {
        currentState.LateUpdate();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdate();

        //if (jumpRequested && jumpCount > 0)
        //{
        //    rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        //    jumpCount--;
        //}
        //jumpRequested = false;

        //if (isGrounded && rb.velocity.y <= 0.05f)
        //{
        //    jumpCount = characterData.charJumpCount;
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void Slide() 
    {
        transform.localScale = new Vector3(2.0f, 0.5f, 1.0f);
        capsuleCollider.direction = CapsuleDirection2D.Horizontal;
    }

    private void ExitSlide()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        capsuleCollider.direction = CapsuleDirection2D.Vertical;
    }

    public void InitState(string stateName)
    {
        currentState = stateDic[stateName];
    }

    public void AddState(string stateName, BaseState state)
    {
        state.SetStateMachine(this);
        stateDic.Add(stateName, state);
    }
    
    public void ChangeState(string stateName)
    {
        currentState.Exit();
        
        currentState = stateDic[stateName];

        currentState.Enter();
    }

    public void InitState<T>(T stateType) where T : Enum
    {
        InitState(stateType.ToString());
    }
    public void AddState<T>(T stateType, BaseState state) where T : Enum
    {
        AddState(stateType.ToString(), state);
    }
    public void ChangeState<T>(T stateType) where T : Enum
    {
        ChangeState(stateType.ToString());
    }
}
