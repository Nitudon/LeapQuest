using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.UI;
using UniRx.Triggers;
using System.Collections;
using System;

/// <summary>
/// シールダーの行動スクリプト
/// </summary>
public class GuardEnemyController : EnemyAbstractController{

    private bool isAttack = false;//突進フラグ
    private ParticleSystem GuardEffect;//ガードエフェクト

    protected override void Start()
    {
        BEHAVE_TIME = 2f;
        GuardEffect = GetComponent<ParticleSystem>();
        base.Start();

        Observable.Timer(TimeSpan.FromSeconds(6))//6秒後に突進
            .Subscribe(_ => EnemyAttack());
    }


    protected override void OnCollisionEnter(Collision collision)
    {
        //プレイヤー攻撃時、突進中なら通常処理、そうでないならガード
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

    //ガードモーション解除
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

    //ガード処理
    private void Guard()
    {
        EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Guard);
        _animator.Play("Guard");
    }

}
