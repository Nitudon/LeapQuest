using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

public class FlyingEnemyController : EnemyAbstractController {

    protected override void Start()
    {
        BEHAVE_TIME = 1f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        Vector3 goal = transform.position - new Vector3(0f,0f,0.2f);
        Vector3 intercept = transform.position += new Vector3(0f,0.05f,-0.1f);

        Vector3[] path = new Vector3[] { intercept,goal};

        transform.DOPath(path, 1f);
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator EnemyDeath()
    {
        return base.EnemyDeath();
    }
}
