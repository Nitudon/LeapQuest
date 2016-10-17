using UnityEngine;
using System.Collections;
using UniRx;
using DG.Tweening;

public class EnemySphere : EnemyAbstractController{

    protected override void Start()
    {
        BEHAVE_TIME = 1.5f;
        base.Start();        
    }

    protected override void EnemyBehave()
    {
        Vector3 interval = pos / 5;
        Vector3 intercept = new Vector3(_transform.position.x + interval.x/2,_transform.position.y + 0.05f,_transform.position.z + interval.z/2);
        Vector3 goal =  intercept + new Vector3(interval.x/2,-0.05f,interval.z/2);

        if (Vector3.Magnitude(goal - Camera.gameObject.transform.position) < 0.42f)
        {
            EnemyAttack();
        }
        else
        {
            transform.DOPath(new Vector3[] { intercept, goal }, 1f, PathType.CatmullRom);
        }
    }

    protected override void EnemyEnd()
    {
        //Vector3 interval = pos;
        //Vector3 intercept = new Vector3(_transform.position.x + interval.x / 2, _transform.position.y + 0.05f, _transform.position.z + interval.z / 2);
        //Vector3 goal = intercept + new Vector3(interval.x / 2, -0.05f, interval.z / 2);

        //transform.DOPath(new Vector3[] { -intercept, -goal }, 2f, PathType.CatmullRom);

        base.EnemyEnd();
    }

    protected override void EnemyAttack()
    {
        Vector3 enemyPosition = _transform.position;

        transform.DOMove(new Vector3(Random.Range(Camera.transform.position.x-0.2f, Camera.transform.position.x + 0.2f),Random.Range(Camera.transform.position.y , Camera.transform.position.y + 0.3f), Camera.transform.position.z), 1f);
        transform.DORotate(new Vector3(60, 180, 0), 1f)
            .OnComplete(() => Destroy(gameObject));
    }
}
