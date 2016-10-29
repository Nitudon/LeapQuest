using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System;

public class BossController :EnemyAbstractController{

    private const int HIT_BREAK_POINT = 5;
    private const int PUNCH_BREAK_POINT = 10;
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
            EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Damaged);
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
        isPunch = true;
        _animator.SetTrigger("Damage");
        _enemyLife--;
        yield return new WaitForSeconds(1f);
        transform.DOMove(bossPosition, 0.5f);
        Debug.Log(_enemyLife);
        if (_enemyLife == 0)
        {
            StartCoroutine(BossDeathCoroutine());
        }
        yield return new WaitForSeconds(1f);
        isPunch = false;
    }

    private IEnumerator BossDeathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyDeath());
        yield break;
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        if (!isBreak && collider.tag == "Rock")
        {
            if (collider.GetComponent<RockController>().isReflect)
            {
                Destroy(collider.gameObject);
                EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Damaged);
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

    protected IEnumerator RockAttack()
    {
        _animator.SetTrigger("Rock");
        yield return new WaitForSeconds(1.2f);
        EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Throw);
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
            StartCoroutine(EnemyAttackCoroutine());
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

    private void ModeChange()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(PUNCH_TIME))
                                       .Where(_ => !_isAttacked)
                                       .Subscribe(_ => EnemyBehave())
                                       .AddTo(gameObject);
    }

    protected override void EnemyAttack()
    {
        throw new NotImplementedException();
    }

    private IEnumerator EnemyAttackCoroutine()
    {
        punchPoint = PUNCH_BREAK_POINT;
        _animator.SetTrigger("Punch");
        yield return new WaitForSeconds(0.6f);
        EnemyManager.Instance.EnemySoundEffect(AudioManager.EnemySE.Tackle);
        if (!isPunch)
        {
            yield return new WaitForSeconds(2f);
            transform.DOMove(bossPosition, 0.5f);
            yield break;
        }
        else
        {
            yield break;
        }
    }



}   
