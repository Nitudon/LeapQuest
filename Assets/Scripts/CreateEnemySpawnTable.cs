using UnityEngine;
using UnityEditor;
using System.Collections;
/// <summary>
/// 敵の出現データのアセット作成スクリプト
///
/// </summary>
public class CreateEnemySpawnTable : MonoBehaviour {
    [MenuItem("Assets/Create/EnemySpawnTable")]
    public static void CreateAsset()
    {
        EnemySpawnTable data = ScriptableObject.CreateInstance<EnemySpawnTable>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/BattleData/" + typeof(EnemySpawnTable) + ".asset");

        AssetDatabase.CreateAsset(data,path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = data;
        
    }

}
