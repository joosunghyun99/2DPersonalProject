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
            return;
        }

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
        //선택 캐릭터 받아오기
        SelectCharacter(GameManager.Instance.selectedCharacter);

        //캐릭터 변수 가져오기
        maxHp = characterData.charHP;
        curHp = maxHp;
        maxJumpCount = characterData.charJumpCount;
        curJumpCount = maxJumpCount;
        jumpPower = characterData.charJumpPower;
        originalRadius = characterData.charMagnetRadius;
        curRadius = originalRadius;
        isInvincible = false;

        //hp 전달
        UIManager.Instance.SetHpSlider(curHp, maxHp);
    }

    // Update is called once per frame
    void Update()
    {
        //땅 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.velocity.y <= 0.05f)
        {
           curJumpCount = maxJumpCount;
        }

        //추락체크
        if (gameObject.transform.position.y < -20.0f)
        {
            GameManager.Instance.GameOver();
        }

        //아이템 끌어당기기
        AttractItem();
    }

    private void FixedUpdate()
    {
        //땅에 닿으면 점프 횟수 초기화
        if (isGrounded && rb.velocity.y <= 0.05f)
        {
            curJumpCount = maxJumpCount;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //땅 체크, 자석 범위 기즈모
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, curRadius);
    }

    public void SelectCharacter(int index) 
    {
        //인덱스가 0보다 작거나 가진 애니메이터 수보다 크면 무시
        if (index < 0 || index >= animControllers.Length)
        {
            return;
        }
        else 
        {
            //아니면 해당하는 인덱스의 애니메이터, 캐릭터 데이터 할당
            anim.runtimeAnimatorController = animControllers[index];
            characterData = characterDataList[index];
        }
    }

    public void GetDamage(int damage) 
    {
        //데미지가 0보다 크면 피격
        if (damage > 0) 
        {
            stateMachine.ChangeState(State.Hit);
        }

        curHp -= damage;

        //hp가 맥스를 넘으면 맥스로 고정
        if (curHp > maxHp) 
        {
            curHp = maxHp; 
        }

        //0이거나 작으면 게임오버
        if (curHp <= 0) 
        {
            GameManager.Instance.GameOver();
        }

        //hp전달
        GameManager.Instance.PlayerHpUpdate(curHp);
    }

    public void AttractItem() 
    {
        //아이템 전부 받아와서
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, curRadius, itemLayer);

        //캐릭터 방향으로 움직이기
        foreach (Collider2D itemCol in items) 
        {
            float moveSpeed = curRadius;
            itemCol.transform.position = Vector2.MoveTowards
                (itemCol.transform.position, gameObject.transform.position, moveSpeed * Time.deltaTime);
        }
    }

    public void Jump() 
    {
        //점프
        curJumpCount--;
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    public void EnterSlide() 
    {
        //콜라이더 조정해서 누운 모양으로 만듬
        capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        capsuleCollider.size = new Vector2(1.8f, 0.9f);
        capsuleCollider.offset = new Vector2(0.3f, -0.5f);
    }

    public void ExitSlide() 
    {
        //콜라이더 원래대로
        capsuleCollider.direction = CapsuleDirection2D.Vertical;
        capsuleCollider.size = new Vector2(0.9f, 1.8f);
        capsuleCollider.offset = new Vector2(0.3f, -0.1f);
    }

    public void ActivateBlink(float duration) 
    {
        //피격시 무적 코루틴 호출
        if (blinkCo != null)
        {
            StopCoroutine(blinkCo);
        }
        blinkCo = StartCoroutine(BlinkCo(duration));
    }

    IEnumerator BlinkCo(float duration)
    {
        float timer = 0.0f;
        //장애물 레이어 충돌 무시
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), true);
        //일정시간 빨갛게 깜빡거리기
        while (timer < duration)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);

            timer += 0.4f;
        }
        //다시 충돌 키기
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), false);
    }

    public void ActivateMagnet(float newRadius, float newDuration) 
    {
        //자석 코루틴 호출
        if (magnetCo != null) 
        {
            StopCoroutine(magnetCo);
        }

        magnetCo = StartCoroutine(MagnetCo(newRadius, newDuration));
    }

    IEnumerator MagnetCo(float radius, float duration) 
    {
        //범위 늘리기
        curRadius = radius;
        //기다리기
        yield return new WaitForSeconds(duration);
        //범위 원래대로
        curRadius = originalRadius;
    }

    public void ActivateInvincible(float duration) 
    {
        //무적 코루틴 호출
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

        //장애물 레이어 충돌 무시
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Obstacle"), true);

        //일정시간 노란색으로 점멸
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

    //버튼 조작용 (미완)
    public void ButtonJump() 
    {
       stateMachine.ChangeState(State.Jump);
    }
    //버튼 조작용 (미완)
    public void ButtonSlide(int num) 
    {
        if (num == 1) { stateMachine.ChangeState(State.Slide); }
        else { stateMachine.ChangeState(State.Run); }
    }

    private class RunState : BaseState
    {
        public RunState(Character owner) : base(owner) { }

        public override void Enter()
        {
            anim.Play("Run");
        }

        public override void Transition()
        {
            //점프
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }
            //슬라이드
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
            //땅이고 점프횟수가 남아있으면
            if (isGrounded && curJumpCount > 0)
            {
                SoundManager.Instance.OnPlayerJump();
                owner.Jump();
                anim.Play("Jump");
            }
        }

        public override void Transition()
        {
            //이단 점프
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }

            //착지하면 달리기
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
            owner.EnterSlide();
            anim.Play("Slide");
        }

        public override void Transition()
        {
            //점프
            if (Input.GetKeyDown(KeyCode.Space) && curJumpCount > 0)
            {
                owner.Jump();
                ChangeState(State.Jump);
            }
            //달리기
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
            SoundManager.Instance.OnPlayerHit();
            owner.ActivateBlink(2.0f);
        }

        public override void Transition()
        {
            //점프, 슬라이드, 달리기 전환
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
