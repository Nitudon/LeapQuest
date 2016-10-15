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

    #region[DamageParameter]
    private const float DAMAGE_DURATION = 1f;//ダメージを受けているときの時間
    private const float DAMAGE_POWER = 0.015f;//ダメージの振動の強さ
    private const int DAMAGE_SHAKE = 22;//ダメージの振動数
    private const int DAMAGE_SHAKE_ANGLERANGE = 20;//ダメージの振動の角度の散らばり
    #endregion

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
            transform.DOKill();
            transform.DOShakePosition(DAMAGE_DURATION, DAMAGE_POWER, DAMAGE_SHAKE, DAMAGE_SHAKE_ANGLERANGE);
            if (breakPoint == 0)
            {
                isBreak = true;
                _animator.SetTrigger("front");
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
        if(breakPoint <= 0)
        _animator.SetTrigger("Rock");
        GameObject attackRock = Instantiate(AttackRock,_transform.position,_transform.rotation) as GameObject;
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }



}   
