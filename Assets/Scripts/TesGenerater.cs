using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class TesGenerater : MonoBehaviour{

    private GameObject[] Enemys;

    [SerializeField]
    private GameObject Boss;

    #region[PrivateParameter]
    private int BattleMode;
    private GameObject camera;
    private Vector3 pos;
    private int EnemyNum;
    private readonly int TEST_GENERATOR_NUM = 3;
    #endregion

    public void OnBattle(int step)
    {
        BattleMode = step;
    }

    // Use this for initialization
    void Start () {
        BattleMode = 0;
        camera = GameObject.Find("Camera");
        EnemyNum = 0;

        Observable.Timer(System.TimeSpan.FromSeconds(0f), System.TimeSpan.FromSeconds(5f))
            .Where(_ => gameObject.active)
            .Subscribe(_ => EnemyGenerate());

    }
	
    private void EnemyGenerate()
    {
        if (BattleMode != 3)
        {
            for (int i = 0; i < TEST_GENERATOR_NUM; ++i)
            {
                Create();
            }
        }

        else
        {
            BossGenerate();
            gameObject.SetActive(false);
        }
    }

    private void BossGenerate()
    {
        GameObject _preBoss;
        pos = new Vector3(camera.transform.position.x,camera.transform.position.y,camera.transform.position.z + 1f);
        _preBoss = Instantiate(Boss, pos, Enemys[0].transform.rotation) as GameObject;
    }

    void Create()
    {
        GameObject _preEnemy;
        pos = new Vector3(Random.Range(camera.transform.position.x-0.4f, camera.transform.position.x + 0.4f),camera.transform.position.y,Random.Range( camera.transform.position.z+0.5f,camera.transform.position.z+1.0f));
        int testNum = Random.Range(0,Enemys.Length);
        _preEnemy = Instantiate(Enemys[testNum], pos, Enemys[0].transform.rotation) as GameObject;
    }

}
