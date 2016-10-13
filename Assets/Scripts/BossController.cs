using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System;

public class BossController :EnemyAbstractController{

    private const int HIT_BREAK_POINT = 3;

    [SerializeField]
    private GameObject AttackRock;

    private int breakPoint;

    private bool isBreak;

    private IntReactiveProperty i; 

    private void OnRockHit()
    {

    }

    protected override void OnAttacked(Collision collision)
    {
        base.OnAttacked(collision);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!isBreak && collision.collider.tag == "Rock")
        {
            breakPoint--;
            if(breakPoint == 0)
            {
                isBreak = true;
                OnRockHit();
            }
        }

        else if (isBreak && collision.collider.tag == "Hand")
        {

        }
    }

    protected override void Start()
    {
        breakPoint = HIT_BREAK_POINT;
        isBreak = false;
        BEHAVE_TIME = 3f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        GameObject attackRock = Instantiate(AttackRock,_transform.position,_transform.rotation) as GameObject;
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }



}   
