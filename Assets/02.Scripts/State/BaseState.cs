using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    private Character character;

    public void SetStateMachine(Character stateMachine)
    {
        this.character = stateMachine;
    }

    protected void ChangeState(string stateName)
    {
        //내부에 저장된 상태머신한테 이 이름 상태로 바꿈
        character.ChangeState(stateName);
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
}
