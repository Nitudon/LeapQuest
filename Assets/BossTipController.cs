using UnityEngine;
using System.Collections;
using System;

public class BossTipController : BossController{

    protected override void Start()
    {
        BEHAVE_TIME = 3f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        StartCoroutine(RockAttack());
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }
}
