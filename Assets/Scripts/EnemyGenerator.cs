using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// EnemyGenerator
/// 敵の生成管理クラス
/// 管理単位は1バトル毎
/// </summary>

public class EnemyGenerator : MonoBehaviour {

    //生成する敵
    private List<GameObject> Enemys;

    //出現時間
    private List<int> TermTimes;

    //出現場所
    private readonly Vector3[] GENERATE_POSITIONS_X = new Vector3[]{
        new Vector3(-0.25f,0f,0.7f),
        new Vector3(0f,0f,0.7f),
        new Vector3(0.25f,0f,0.7f)
    };

    private readonly Vector3[] GENERATE_POSITIONS_Z = new Vector3[]{
        new Vector3(0f,0f,0f),
        new Vector3(0f,0f,0.5f),
        new Vector3(0f,0f,1f)
    };

    private readonly Vector3 BOSS_POSITION = new Vector3(-1.24f,-3.6027f,0.91f);

    //敵の情報のイニシャライズ
    public void Init()
    {
        Enemys = new List<GameObject>();
        Enemys.Add(Resources.Load("Enemys/Slime") as GameObject);
        Enemys.Add(Resources.Load("Enemys/MetalSlime") as GameObject);
        Enemys.Add(Resources.Load("Enemys/Shielder") as GameObject);
        Enemys.Add(Resources.Load("Enemys/Flydevil") as GameObject);
        Enemys.Add(Resources.Load("Enemys/Boss") as GameObject);
    }

    //バトル開始時に敵の生成システムを立ち上げる
    public void BattleStart(EnemySpawnTable data)
    {
        //出現場所の基準となるplayer
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //出現時間の設定
        TermTimes = new List<int>();
        for(int i = 0; i < data.EnemyTerms.Count; ++i)
        {
            TermTimes.Add(data.EnemyTerms[i]._time);
        }

        //敵の生成Disposer、設定した時間ごとに生成
        EnemyGenerateObservable(data)
            .Subscribe(x => EnemyGenerate(data.EnemyTerms[TermTimes.IndexOf((int)x)]._enemyEntrys.ToArray(),player));
    }

    //出現時間のObservable、時間を監視し、指定の時間なら生成、生成が終わった時購読を停止
    IObservable<long> EnemyGenerateObservable(EnemySpawnTable data)
    {
        var observable =
            Observable.Timer(System.TimeSpan.FromSeconds(0f), System.TimeSpan.FromSeconds(1f))//1秒単位の監視
            .Take(data.EnemyTerms[data.EnemyTerms.Count-1]._time + 1)//最後の生成が終わったとき購読を停止
            .Where(x => TermTimes.Contains((int)x));//指定した時間かどうかの監視
            
        return observable;
    }

    //敵の生成、出現情報（EnemySpawn）から読み取り生成する
    private void EnemyGenerate(EnemySpawn[] spawns,GameObject player)
    {
        //1回に生成する数だけ情報を取得、生成
        foreach (EnemySpawn spawn in spawns)
        {
            //情報の取得
            Vector3 pos = player.transform.position + GENERATE_POSITIONS_X[(int)spawn.placeX] + GENERATE_POSITIONS_Z[(int)spawn.placeZ];
            GameObject enemy = Enemys[(int)spawn.enemy];
            //生成
            if (spawn.enemy == EnemySpawn.EnemyKinds.boss)
            {
                Instantiate(enemy, BOSS_POSITION, enemy.transform.rotation);
            }
            else
            {
                Instantiate(enemy, pos, enemy.transform.rotation);
            }
        }
    }

}
