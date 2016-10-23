using UnityEngine;
using System.Collections;

/// <summary>
/// 敵の出現情報
/// 種類：Kind
/// 出現ポイント：Place
/// 出現数：Number
/// </summary>

    [System.Serializable]
public class EnemySpawn {

    #region[enumList]

    public enum spawnPlace_X
    {
        left,
        center,
        right,
        random
    }

    public enum spawnPlace_Z
    {
        front,
        middle,
        back,
        random
    }

    public enum EnemyKinds
    {
        slime,
        metalslime,
        shielder,
        flydevil,
        boss
    }

    #endregion

    #region[SpawnParameter]
    [SerializeField] EnemyKinds Enemy;//敵の種類
    [SerializeField] spawnPlace_X PlaceX;//出現場所
    [SerializeField] spawnPlace_Z PlaceZ;
    #endregion

    //アクセス
    public EnemyKinds enemy { get { return Enemy; } }
    public spawnPlace_X placeX { get { return PlaceX; } }
    public spawnPlace_Z placeZ { get { return PlaceZ; } }

}
