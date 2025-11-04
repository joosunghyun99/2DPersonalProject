using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<string, BaseState> stateDic = new Dictionary<string, BaseState>();

    private BaseState curState;
    void Start()
    {
        if (curState != null)
        {
            curState.Enter();
        }
        else 
        {
            Debug.Log("curState null");
        }
    }

    void Update()
    { 
        curState.Update();
        
        curState.Transition();
    }

    private void LateUpdate()
    {
        curState.LateUpdate();
    }
    private void FixedUpdate()
    {
        curState.FixedUpdate();
    }
    public void InitState(string stateName)
    {
        curState = stateDic[stateName];
    }
    
    public void AddState(string stateName, BaseState state)
    {
        state.SetStateMachine(this);
        stateDic.Add(stateName, state);
    }
    
    public void ChangeState(string stateName)
    {
        curState.Exit();
        
        curState = stateDic[stateName];

        curState.Enter();
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

public class BaseState
{
    private StateMachine stateMachine;

    protected Character owner;

    protected Transform transform { get { return owner.transform; } }
    protected Rigidbody2D rb { get { return owner.rb; } }
    protected SpriteRenderer spriteRenderer { get { return owner.spriteRenderer; } }
    protected CapsuleCollider2D capsuleCollider { get { return owner.capsuleCollider; } }
    protected Animator anim { get { return owner.anim; } }

    protected int maxHp { get { return owner.maxHp; } }
    protected int curHp { get { return owner.curHp; } }
    protected int maxJumpCount { get { return owner.maxJumpCount; } }
    protected int curJumpCount { get { return owner.curJumpCount; } }
    protected float jumpPower { get { return owner.jumpPower; } }
    protected float originalRadius { get { return owner.originalRadius; } }
    protected float curRadius { get { return owner.curRadius; } }

    protected bool isGrounded { get { return owner.isGrounded; } }
    protected bool jumpRequested { get { return owner.jumpRequested; } }
    protected bool isInvincible { get { return owner.isInvincible; } }

    public BaseState(Character owner) { this.owner = owner; }

    public void SetStateMachine(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
   
    protected void ChangeState(string stateName)
    {
        
        stateMachine.ChangeState(stateName);
    }

    protected void ChangeState<T>(T stateType) where T : Enum
    {
        ChangeState(stateType.ToString());
    }
 
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void Transition() { }

    public virtual void OnCollision(Collider2D collision) { }
}
