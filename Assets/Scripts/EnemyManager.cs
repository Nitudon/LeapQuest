using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 敵の管理、数や出現など
/// </summary>

public class EnemyManager : MonoBehaviour {

    //instance
    private static EnemyManager _instance;

    //敵の生成クラス
    [HideInInspector]
    public EnemyGenerator _enemyGenerator { get; private set;}

    //出現データ
    private EnemySpawnTable _battleData;
    //出現データをバトル毎に分割したもの
    private EnemySpawnTerm[] _battleTerms;
    
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
        _battleData = Resources.Load("BattleData/Battle1") as EnemySpawnTable;
        _battleTerms = _battleData.EnemyTerms.ToArray();
    }

    //バトル開始時の処理
    public void BattleStart()
    {
        _enemyGenerator.BattleStart(_battleData);
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
