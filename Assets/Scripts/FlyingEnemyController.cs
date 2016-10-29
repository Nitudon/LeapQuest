using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

/// <summary>
/// ふわふわデビルの行動スクリプト
/// </summary>

public class FlyingEnemyController : EnemyAbstractController {

    protected override void Start()
    {
        BEHAVE_TIME = 1.7f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        //ふわふわと突進
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMove(transform.position + new Vector3(0f,0.02f,-0.12f),0.8f));
        seq.Append(transform.DOMove(transform.position + new Vector3(0f, 0f, -0.24f), 0.8f));
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
