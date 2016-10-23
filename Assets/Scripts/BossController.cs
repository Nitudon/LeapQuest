using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System;

public class BossController :EnemyAbstractController{

    private const int HIT_BREAK_POINT = 5;
    private const int PUNCH_TIME = 7;

    [SerializeField]
    private GameObject AttackRock;

    private int breakPoint;
    private int punchPoint;
    private Vector3 bossPosition;
    private bool isBreak;
    private bool isPunch;

    protected override void OnCollisionEnter(Collision collision)
    {
        if(!isPunch && _enemyLife > 0 &&  punchPoint > 0 && collision.gameObject.tag == "Hand" && _animator.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            punchPoint--;
            OnAttackedShake();
            if (punchPoint == 0)
            {
                StartCoroutine(PunchCoroutine());
            }
        }
    }

    protected override IEnumerator EnemyDeath()
    {
        GameObject _particle = Instantiate(DeathParticle, _transform.position, DeathParticle.transform.rotation) as GameObject;
        EnemyDestroy();
        yield return new WaitForSeconds(3f);
        Destroy(_particle);
        yield break;
    }

    private IEnumerator PunchCoroutine()
    {
        isPunch = false;
        _animator.SetTrigger("Damage");
        punchPoint = HIT_BREAK_POINT;
        _enemyLife--;
        yield return new WaitForSeconds(1f);
        transform.DOMove(bossPosition, 0.5f);
        Debug.Log(_enemyLife);
        if (_enemyLife == 0)
        {
            StartCoroutine(BossDeathCoroutine());
        }
        yield return new WaitForSeconds(1f);
        isPunch = true;
    }

    private IEnumerator BossDeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyDeath());
        yield break;
    }

    protected override void OnAttacked(Collision collision)
    {
        base.OnAttacked(collision);
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (!isBreak && collider.tag == "Rock")
        {
            if (collider.GetComponent<RockController>().isReflect)
            {
                Destroy(collider.gameObject);
                breakPoint--;
                transform.DOKill();
                OnAttackedShake();
                if (breakPoint == 0)
                {
                    punchPoint = HIT_BREAK_POINT;
                    isBreak = true;
                }
            }
        }

        if(isBreak && collider.tag == "Player")
        {
            PlayerUIManager.Instance.LifeAffect(ATTACK_DAMAGE);
        }

    }

    protected override void Start()
    {
        isPunch = false;
        bossPosition = transform.position;
        ATTACK_DAMAGE = 3;
        breakPoint = HIT_BREAK_POINT;
        isBreak = false;
        BEHAVE_TIME = 5f;
        base.Start();
    }

    private IEnumerator RockAttack()
    {
        _animator.SetTrigger("Rock");
        yield return new WaitForSeconds(1.2f);
        GameObject attackRock = Instantiate(AttackRock, _transform.position + new Vector3(-0.2f, 0.1f, 0f), _transform.rotation) as GameObject;
        yield break;
    }

    protected override void EnemyBehave()
    {
        if (!isBreak)
        {
            StartCoroutine(RockAttack());
        }
        else
        {
            EnemyAttack();
        }
    }

    private IObservable<long> BreakObservable()
    {
        var observable = Observable.EveryUpdate()
            .Where(_ => isBreak);

        return observable;
    }

    protected override IDisposable EnemyRoutineDisposable()
    {
        var disposable = Observable.Interval(System.TimeSpan.FromSeconds(BEHAVE_TIME))
           .Where(_ => !_isAttacked)
           .TakeUntil(BreakObservable())
           .Subscribe(_ => EnemyBehave(),
                      () => Observable.Interval(System.TimeSpan.FromSeconds(PUNCH_TIME))
                                       .Where(_ => !_isAttacked)
                                       .Subscribe(_ => EnemyBehave())
                                       .AddTo(gameObject)
           );

        return disposable;
    }

    protected override void EnemyAttack()
    {
        _animator.SetTrigger("Punch");
    }



}   
