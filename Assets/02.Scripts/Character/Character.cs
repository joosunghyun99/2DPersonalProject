using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State { Run, Slide, Jump, Hit }

    [Header("PlayerData")]
    [SerializeField] private CharacterData[] characterDataList;
    public CharacterData characterData;
    private StateMachine stateMachine;
    private State currentState;

    private Dictionary<string, BaseState> stateDic = new Dictionary<string, BaseState>();

    private Coroutine magnetCo;
    private Coroutine invincibleCo;
    private Coroutine blinkCo;

    [Header("PlayerAnimator")]
    [SerializeField] private RuntimeAnimatorController[] animControllers;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider;
    public Animator anim;

    [Header("Parameter")]
    public int maxHp;
    public int curHp;
    public int maxJumpCount;
    public int curJumpCount;
    public float jumpPower;
    public float originalRadius;
    public float curRadius;
    [SerializeField] private LayerMask itemLayer;

    public bool isGrounded;
    public bool jumpRequested;
    public bool isInvincible;

    private void Awake()
    {
        if (characterDataList == null) 
        {
            Debug.Log("CharacterData null");
            return;
        }

        Debug.Log($"GameManager.Instance: {GameManager.Instance}");

        //캐릭터 컴포넌트
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();

        //스테이트 머신
        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.AddState(State.Run, new RunState(this));
        stateMachine.AddState(State.Jump, new JumpState(this));
        stateMachine.AddState(State.Slide, new SlideState(this));
        stateMachine.AddState(State.Hit, new HitState(this));
        stateMachine.InitState(State.Run);
    }

    private void Start()
    {
        SelectCharacter(GameManager.Instance.selectedCharacter);

        //캐릭터 변수
        maxHp = characterData.charHP;
        curHp = maxHp;
        maxJumpCount = characterData.charJumpCount;
        curJumpCount = maxJumpCount;
        jumpPower = characterData.charJumpPower;
        originalRadius = characterData.charMagnetRadius;
        curRadius = originalRadius;
        isInvincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.velocity.y <= 0.05f)
        {
           curJumpCount = maxJumpCount;
        }

        if (gameObject.transform.position.y < -20.0f)
        {
            GameManager.Instance.GameOver();
        }

        AttractItem();
    }

    private void FixedUpdate()
    {
        if (jumpRequested && curJumpCount > 0)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            curJumpCount--;
        }
        jumpRequested = false;

        if (isGrounded && rb.velocity.y <= 0.05f)
        {
            curJumpCount = maxJumpCount;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, curRadius);
    }

    public void SelectCharacter(int index) 
    {
        if (index < 0 || index >= animControllers.Length)
        {
            return;
        }
        else 
        {
            Debug.Log(index);
            anim.runtimeAnimatorController = animControllers[index];
            characterData = characterDataList[index];
        }
    }

    public void GetDamage(int damage) 
    {
        if (damage > 0) 
        {
            stateMachine.ChangeState(State.Hit);
        }

        curHp -= damage;

        if (curHp > maxHp) 
        {
            curHp = maxHp; 
        }

        if (curHp <= 0) 
        {
            GameManager.Instance.GameOver();
        }

        GameManager.Instance.PlayerHpUpdate(curHp);
    }

    public void AttractItem() 
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, curRadius, itemLayer);

        foreach (Collider2D itemCol in items) 
        {
            float moveSpeed = curRadius;
            itemCol.transform.position = Vector2.MoveTowards
                (itemCol.transform.position, gameObject.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    public void Jump() 
    {
        curJumpCount--;
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    public void EnterSlide() 
    {
       //transform.localScale = new Vector3(2.0f, 0.5f, 1.0f);
        capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        capsuleCollider.size = new Vector2(1.8f, 0.9f);
        capsuleCollider.offset = new Vector2(0.3f, -0.5f);
    }

    public void ExitSlide() 
    {
        //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        capsuleCollider.direction = CapsuleDirection2D.Vertical;
        capsuleCollider.size = new Vector2(0.9f, 1.8f);
        capsuleCollider.offset = new Vector2(0.3f, -0.1f);
    }

    public void ActivateBlink(float duration) 
    {
        if (blinkCo != null)
        {
            StopCoroutine(blinkCo);
        }
        blinkCo = StartCoroutine(BlinkCo(duration));
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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), false);
    }

    public void ActivateMagnet(float newRadius, float newDuration) 
    {
        if (magnetCo != null) 
        {
            StopCoroutine(magnetCo);
        }

        magnetCo = StartCoroutine(MagnetCo(newRadius, newDuration));
    }

    IEnumerator MagnetCo(float radius, float duration) 
    {
        curRadius = radius;

        yield return new WaitForSeconds(duration);

        curRadius = originalRadius;
    }

    public void ActivateInvincible(float duration) 
    {
        if (invincibleCo != null) 
        {
            StopCoroutine(invincibleCo);
        }

        invincibleCo = StartCoroutine(InvincibleCo(duration));
    }

    IEnumerator InvincibleCo(float duration) 
    {
        isInvincible = true;
        float timer = 0.0f;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), true);

        while (timer < duration)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.white;
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
            anim.Play("Run");
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }

            if (Input.GetKeyDown(KeyCode.S)) 
            {
                ChangeState(State.Slide);
            }
        }
    }

    private class JumpState : BaseState
    {
        public JumpState(Character owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Jump");
            owner.Jump();
            anim.Play("Jump");
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }

            if (isGrounded && rb.velocity.y <= 0.05f)
            {
                ChangeState(State.Run);
            }
        }
    }

    private class SlideState : BaseState
    {
        public SlideState(Character owner) : base(owner) { }
        public override void Enter()
        {
            Debug.Log("Slide");
            owner.EnterSlide();
            anim.Play("Slide");
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }
            else if (Input.GetKeyUp(KeyCode.S)) 
            {
                ChangeState(State.Run);
            }
        }

        public override void Exit()
        {
            owner.ExitSlide();
        }
    }

    private class HitState : BaseState
    {
        public HitState(Character owner) : base(owner) { }

        public override void Enter() 
        {
            Debug.Log("Hit");
            SoundManager.Instance.OnPlayerHit();
            owner.ActivateBlink(2.0f);
        }

        public override void Transition()
        {
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
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
