﻿using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class FlyingEnemyController : EnemyAbstractController {

    protected override void Start()
    {
        BEHAVE_TIME = 1.7f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(transform.position + new Vector3(0f,0.02f,-0.05f),0.8f));
        seq.Append(transform.DOMove(transform.position + new Vector3(0f, 0f, -0.1f), 0.8f));
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator EnemyDeath()
    {
        transform.DOKill();
        return base.EnemyDeath();
    }
}
