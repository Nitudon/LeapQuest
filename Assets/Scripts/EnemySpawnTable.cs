using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 敵の出現情報、1回のバトルの情報
/// </summary>

[System.Serializable]
public class EnemySpawnTable:ScriptableObject{

    public List<EnemySpawnTerm> EnemyTerms;//行われる生成タームの集合
    public List<EnemySpawnTerm> _enemyTerm { get {return EnemyTerms; } }//アクセス

//    #region[InspectorScript]
//#if UNITY_EDITOR
//    //Inspector拡張
//    [CustomEditor(typeof(EnemySpawnTable),true)]
//    public sealed class EnemySpawnTableEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//            //target
//            EnemySpawnTable table = target as EnemySpawnTable;
//            if(GUILayout.Button("Add Enemy"))
//            {
//                table.EnemyTerms.Add(new EnemySpawnTerm());
//            }

//        }
//    }
//#endif

  //  #endregion
}
