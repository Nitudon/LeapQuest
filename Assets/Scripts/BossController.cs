using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System;

public class BossController :EnemyAbstractController{

    [SerializeField]
    private GameObject AttackRock;

    protected override void Start()
    {
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
