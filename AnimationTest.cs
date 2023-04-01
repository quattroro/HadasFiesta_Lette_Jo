using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
/////////////////////////////////////////////////////////////////////

//사용되지 않음

public class AnimationTest : MonoBehaviour
{
    public enum States
    {
        Idle,
        Walk,
        Move,
        Attack,
        Rolling,
        Guard,
        GuardStun,
        OutOfControl,
    }

    

    public AnimationController animator;
    public RuntimeAnimatorController stateanimations;
    public MyStateMachine.StateMachine<States, MyStateMachine.Drive> fsm;

    void Start()
    {
        animator = GetComponent<AnimationController>();
        fsm = new MyStateMachine.StateMachine<States, MyStateMachine.Drive>(this);
        //fsm.InitMachineRunner();
    }

    void Idle_Enter()
    {
        Debug.Log("Idle Enter");
    }

    IEnumerator Idle_Exit()
    {
        Debug.Log("Idle Exit");
        yield return new WaitForSeconds(2.0f);
    }

    void Idle_Update()
    {
        Debug.Log("Idle Update");
    }

    void Walk_Enter()
    {
        Debug.Log("Walk Enter");
    }

    void Walk_Exit()
    {
        Debug.Log("Walk Exit");
    }

    bool flag = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            fsm.ChangeState(States.Idle);
            //animator.Play(stateanimations);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            fsm.ChangeState(States.Walk);
            //flag = !flag;
            //animator.SetBool("Run", flag);

        }
        

    }
}
