﻿using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System;

public class BossController :EnemyAbstractController{

    private const int HIT_BREAK_POINT = 3;
    private const int PUNCH_TIME = 7;

    [SerializeField]
    private GameObject AttackRock;

    private int breakPoint;
    private int punchPoint;
    private Vector3 bossPosition;
    private bool isBreak;

    #region[DamageParameter]
    private const float DAMAGE_DURATION = 1f;//ダメージを受けているときの時間
    private const float DAMAGE_POWER = 0.015f;//ダメージの振動の強さ
    private const int DAMAGE_SHAKE = 22;//ダメージの振動数
    private const int DAMAGE_SHAKE_ANGLERANGE = 20;//ダメージの振動の角度の散らばり
    #endregion

    protected override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Hand" && (_animator.GetCurrentAnimatorStateInfo(0).IsName("Punch") || _animator.GetCurrentAnimatorStateInfo(0).IsName("FaintPunch")))
        {
            punchPoint--;
            transform.DOShakePosition(DAMAGE_DURATION, DAMAGE_POWER, DAMAGE_SHAKE, DAMAGE_SHAKE_ANGLERANGE);
            if (punchPoint == 0)
            {
                _animator.SetTrigger("Damage");
                _enemyLife--;
                transform.DOMove(bossPosition, 0.5f);
                Debug.Log(_enemyLife);
                punchPoint = HIT_BREAK_POINT;
            }
        }
    }

    protected override void OnAttacked(Collision collision)
    {
        base.OnAttacked(collision);
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (!isBreak && collider.tag == "Rock")
        {
            if (collider.GetComponent<RockController>().isReflect)
            {
                Destroy(collider.gameObject);
                breakPoint--;
                transform.DOKill();
                transform.DOShakePosition(DAMAGE_DURATION, DAMAGE_POWER, DAMAGE_SHAKE, DAMAGE_SHAKE_ANGLERANGE);
                if (breakPoint == 0)
                {
                    punchPoint = HIT_BREAK_POINT;
                    isBreak = true;
                }
            }
        }

        if(isBreak && collider.tag == "Player")
        {
            PlayerUIManager.Instance.LifeAffect(ATTACK_DAMAGE);
        }

    }

    protected override void Start()
    {
        bossPosition = transform.position;
        ATTACK_DAMAGE = 3;
        breakPoint = HIT_BREAK_POINT;
        isBreak = false;
        BEHAVE_TIME = 5f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        if (!isBreak)
        {
            _animator.SetTrigger("Rock");
            GameObject attackRock = Instantiate(AttackRock, _transform.position + new Vector3(-0.2f, 0.1f, 0f), _transform.rotation) as GameObject;
        }
        else
        {
            EnemyAttack();
        }
    }

    private IObservable<long> BreakObservable()
    {
        var observable = Observable.EveryUpdate()
            .Where(_ => isBreak);

        return observable;
    }

    protected override IDisposable EnemyRoutineDisposable()
    {
        var disposable = Observable.Interval(System.TimeSpan.FromSeconds(BEHAVE_TIME))
           .Where(_ => !_isAttacked)
           .TakeUntil(BreakObservable())
           .Subscribe(_ => EnemyBehave(),
                      () => Observable.Interval(System.TimeSpan.FromSeconds(PUNCH_TIME))
                                       .Where(_ => !_isAttacked)
                                       .Subscribe(_ => EnemyBehave())
                                       .AddTo(gameObject)
           );

        return disposable;
    }

    protected override void EnemyAttack()
    {
        _animator.SetTrigger("Punch");
    }



}   
