using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 敵の管理、数や出現など
/// </summary>

public class EnemyManager : MonoBehaviour {

    //バトル数
    private const int BATTLE_NUMBER = 5;

    //instance
    private static EnemyManager _instance;
    
    //敵の生成クラス
    [HideInInspector]
    public EnemyGenerator _enemyGenerator { get; private set;}

    //出現データ
    private EnemySpawnTable[] _battleData = new EnemySpawnTable[BATTLE_NUMBER];

    //倒した数の監視、バトル遷移用
    [HideInInspector]
    public IntReactiveProperty EnemyNum;

    //Singleton
    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            Debug.Log("There are two Singleton objects:" + gameObject.name.ToString());
        }
    }

    //イニシャライズ、敵の情報の読み込み
    void Init()
    {
        _enemyGenerator = new EnemyGenerator();
        _enemyGenerator.Init();
        for(int i = 0; i < BATTLE_NUMBER; ++i)
        {
            _battleData[i] = Resources.Load("BattleData/Battle" + (i+1).ToString()) as EnemySpawnTable;
        }

        EnemyNum = new IntReactiveProperty();

        //戦闘終了処理
        EnemyNum
            .Skip(1)
            .Where(x => x == 0)
            .Delay(System.TimeSpan.FromSeconds(2f))
            .Subscribe(_ => PlayerUIManager.Instance.OnPlayerWalk());
    }

    public void EnemyDestroy()
    {
        EnemyNum.Value--;
    }

    //バトル開始時の処理
    public void BattleStart(int step)
    {
        int enemyNum = 0;
        foreach(EnemySpawnTerm term in _battleData[step].EnemyTerms)
        {
            enemyNum += term._enemyEntrys.Count;
        }
        _enemyGenerator.BattleStart(_battleData[step]);

        EnemyNum.Value = enemyNum;
    }

    //get_instance
    public static EnemyManager Instance
    {
        get
        {
            if(_instance == null)
            {
                //立ち上げ
                EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
                enemyManager.Init();
                _instance = enemyManager;
            }
            return _instance;
        }
    }
}
