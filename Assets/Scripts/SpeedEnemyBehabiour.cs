using UnityEngine;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class SpeedEnemyBehabiour : EnemyAbstractController{

    private const float OUT_MOVE_DISTANCE = 0.8f;
    private bool isGround = false;

    protected override void Start()
    {
        BEHAVE_TIME = 2f;
        base.Start();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    protected override void EnemyBehave()
    {
        Vector3[] MoveVectors = new Vector3[] { new Vector3(Camera.transform.position.x+OUT_MOVE_DISTANCE, transform.position.y, transform.position.z - 0.01f), new Vector3(Camera.transform.position.x - OUT_MOVE_DISTANCE, transform.position.y, transform.position.z - 0.02f) };

        _transform.DOPath(MoveVectors, 1.8f);

    }

    protected override void EnemyAttack()
    {
        Vector3 enemyPosition = _transform.position;

        transform.DOMove(Camera.transform.position, 1f);
        transform.DORotate(new Vector3(60, 180, 0), 1f)
            .OnComplete(() => Destroy(gameObject));
    }

}
