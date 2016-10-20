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
        transform.DOMoveZ(transform.position.z - 0.05f, 1f);
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerator EnemyDeath()
    {
        GetComponent<Rigidbody>().useGravity = true;
        return base.EnemyDeath();
    }

}
