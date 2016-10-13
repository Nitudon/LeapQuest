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
        BEHAVE_TIME = 6f;
        GuardEffect = GetComponent<ParticleSystem>();
        base.Start();
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
        isAttack = true;
        _animator.Play("Attack");
       EnemyAttack();
    }

    protected override void EnemyAttack()
    {
        _transform.DOMoveZ(Camera.transform.position.z, 1f)
            .OnComplete(() => Destroy(gameObject));
    }

    private void Guard()
    {
        _animator.Play("Guard");
    }

}
