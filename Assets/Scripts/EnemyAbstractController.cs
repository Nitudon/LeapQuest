using UnityEngine;
using System.Collections;
using DG.Tweening;
using UniRx;

[RequireComponent(typeof(Animator),typeof(Rigidbody),typeof(ConstantForce))]
/*
 * 敵の基底クラス
 * Attacked:被攻撃
 * EnemyAttack:攻撃
 * Behave:行動
 * Death:消滅
*/
public abstract class EnemyAbstractController : MonoBehaviour {

    #region[SerializeMembers]
    [SerializeField]
    protected GameObject DeathParticle;

    [SerializeField]
    protected GameObject AttackParticle;

    [SerializeField]
    private int EnemyLife;

    [SerializeField]
    private float DeathTime;
    #endregion

    #region[Parameter]
    protected GameObject Camera;
    protected float deathTime
    {
        get { return DeathTime; }
    }

    protected bool _isAttacked
    {
        get { return IsAttacked; }
    }

    private const float INVICIBLE_TIME = 0.5f;
    protected readonly Vector3 DEATH_SMASH_DISTANCE = new Vector3(0f,0.7f,0.2f);
    private bool IsAttacked;
    private bool IsDeath;
    #region[Protected Parameter]
    protected float BEHAVE_TIME = 2f;
    protected int ATTACK_DAMAGE = 1;
    protected Animator _animator;
    protected Rigidbody _rigitbody;
    protected Collider _collider;
    protected ConstantForce _constantForce;
    protected Transform _transform;
    protected int _enemyLife;
    protected Vector3 pos;
    #endregion
    #endregion

    /** 衝突時の処理*/
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!IsAttacked && collision.gameObject.tag == "Hand")
        {
            gameObject.transform.DOKill();
            IsAttacked = true;
            OnAttacked(collision);
            StartCoroutine(WaitInvicible());
        }

    }

    protected virtual void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            PlayerUIManager.Instance.LifeAffect(ATTACK_DAMAGE);
        }
    }

    /** 消滅時の処理起動*/
    protected virtual void StartDeathCoroutine()
    {
        StartCoroutine(EnemyDeath());
    }

    /** 多段ヒット対策の無敵時間*/
    private IEnumerator WaitInvicible()
    {
        yield return new WaitForSeconds(INVICIBLE_TIME);
        IsAttacked = false;
        yield break;
    }

    /** 攻撃されたときの処理*/
    protected virtual void OnAttacked(Collision collision)
    {
        if (collision.gameObject.tag == "Hand")
        {
            _enemyLife--;
        }

        if (_enemyLife == 0)
        {
            _rigitbody.velocity = Vector3.zero;
            StartCoroutine(EnemyDeath());
        }
    }

    /** 敵の行動ルーチン*/
    protected abstract void EnemyBehave();

    /** 敵の攻撃ルーチン*/
    protected abstract void EnemyAttack();

    /** 敵のターン終了ルーチン*/
    protected virtual void EnemyEnd()
    {
        Destroy(gameObject);
    }

    public void EndTurn()
    {
        EnemyEnd();
    }

    /** 消滅時の処理*/
    protected virtual IEnumerator EnemyDeath()
    {
        _collider.enabled = false;
        _rigitbody.AddForce(DEATH_SMASH_DISTANCE);
        _constantForce.enabled = true;
        yield return new WaitForSeconds(deathTime);
        GameObject _particle = Instantiate(DeathParticle, _transform.position, DeathParticle.transform.rotation) as GameObject;
        Destroy(gameObject);
        yield return new WaitForSeconds(3f);
        Destroy(_particle);
        yield break;
    }

	// Use this for initialization
	protected virtual void Start () {
        IsAttacked = IsDeath = false;
        _animator = GetComponent<Animator>();
        _enemyLife = EnemyLife;
        Camera = GameObject.Find("Camera");
        _collider = GetComponent<BoxCollider>();
        _rigitbody = GetComponent<Rigidbody>();
        _constantForce = GetComponent<ConstantForce>();
        _transform = transform;
        pos = Camera.transform.position - transform.position;

        Observable.Interval(System.TimeSpan.FromSeconds(BEHAVE_TIME))
            .Where(_ => !_isAttacked)
            .Subscribe(_ => EnemyBehave())
            .AddTo(gameObject);

    }

}
