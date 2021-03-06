﻿using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using System.Collections;

/// <summary>
/// メタルスライムの行動スクリプト
/// </summary>
public class SpeedEnemyBehabiour : EnemyAbstractController{

    private const float OUT_MOVE_DISTANCE = 0.8f;//移動する横幅
    private bool isGround = false;//接地判定

    protected override void Start()
    {
        BEHAVE_TIME = 1.5f;
        base.Start();
    }

    protected override void EnemyBehave()
    {
        //ジグザグに突進
        Vector3[] MoveVectors = new Vector3[] { new Vector3(Camera.transform.position.x+OUT_MOVE_DISTANCE, transform.position.y, transform.position.z - 0.13f), new Vector3(Camera.transform.position.x - OUT_MOVE_DISTANCE, transform.position.y, transform.position.z - 0.26f) };

        _transform.DOPath(MoveVectors, 1.3f);

    }

    protected override void EnemyAttack()
    {
        Vector3 enemyPosition = _transform.position;

        transform.DOMove(Camera.transform.position, 1f);
        transform.DORotate(new Vector3(60, 180, 0), 1f)
            .OnComplete(() => Destroy(gameObject));
    }

}
