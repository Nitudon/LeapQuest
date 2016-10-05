using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// 敵の出現情報
/// 種類：Kind
/// 出現ポイント：Place
/// 出現数：Number
/// </summary>


public class EnemySpawn : MonoBehaviour {
    /// <summary>
    /// 内部処理
    /// 出現情報の処理
    /// </summary>
    #region[enumList]
    public enum spawnPlace_X
    {
        center,
        left,
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
    #endregion

    #region[SpawnParameter]
    [SerializeField] GameObject Kind;
    [SerializeField] spawnPlace_X PlaceX;
    [SerializeField] spawnPlace_Z PlaceZ;
    [SerializeField] int Number;
    #endregion

    #region[GetterSetter]
    public GameObject kind { get { return Kind; } }
    public spawnPlace_X placeX { get { return PlaceX; } }
    public spawnPlace_Z placeZ { get { return PlaceZ; } }
    public int number { get { return Number; } }
    #endregion

    #region[InspectorScript]
#if UNITY_EDITOR
    //Inspector拡張
    [CustomEditor(typeof(EnemySpawn))]
    public class EnemySpawnEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //target
            EnemySpawn spawn = target as EnemySpawn;

            spawn.Kind = EditorGUILayout.ObjectField("敵の種類",spawn.Kind,typeof(GameObject),false) as GameObject;
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("出現位置");
            spawn.PlaceX = (spawnPlace_X)EditorGUILayout.EnumPopup("X座標",(spawnPlace_X)spawn.PlaceX);
            spawn.PlaceZ = (spawnPlace_Z)EditorGUILayout.EnumPopup("Z座標", (spawnPlace_Z)spawn.PlaceZ);


        }
    }
#endif

    #endregion
}
