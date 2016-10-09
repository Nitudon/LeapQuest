using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// 敵の出現情報、一回に生成する全ての情報とその時間
/// </summary>
[CanEditMultipleObjects]
[System.Serializable]
public class EnemySpawnTerm 
{
    //出現時間
    [SerializeField]
    private int Time;

    //出現する敵の情報
    [SerializeField]
    private List<EnemySpawn> EnemyEntrys;

    //アクセス
    public int _time { get { return Time; } }
    public List<EnemySpawn> _enemyEntrys { get { return EnemyEntrys; } }

  
}
