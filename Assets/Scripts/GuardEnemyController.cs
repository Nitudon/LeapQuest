using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;
using System.Collections;
using System;

public class GuardEnemyController : EnemyAbstractController{

    private bool isAttack = false;
    private ParticleSystem GuardEffect;

    protected override void Start()
    {
        BEHAVE_TIME = 2f;
        GuardEffect = GetComponent<ParticleSystem>();
        base.Start();

        Observable.Timer(TimeSpan.FromSeconds(6))
            .Subscribe(_ => EnemyAttack());
    }


    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hand")
        {
            if (isAttack)
            {
                base.OnCollisionEnter(collision);
            }

            else
            {
                Guard();
            }

        }
       
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Hand")
        {
            _animator.Play("GuardOut");
        }
    }

    protected override void EnemyBehave()
    {
        if (!isAttack)
        {
            transform.DOMoveZ(transform.position.z - 0.05f, 1f);
        }
    }

    protected override void EnemyAttack()
    {
        transform.DOKill();
        isAttack = true;
        _transform.DOMoveZ(Camera.transform.position.z, 1f)
            .OnComplete(() => Destroy(gameObject));
    }

    private void Guard()
    {
        _animator.Play("Guard");
    }

}
