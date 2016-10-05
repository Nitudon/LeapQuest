using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UniRx;

public class CharacterManager : MonoBehaviour {
    static CharacterManager _instance;

    public List<EnemyAbstractController> EnemyList { get; private set; }
    public int PlayerLife { get; private set; }
    private UIManager _UIManager;
    void Awake()
    {
        _instance = new CharacterManager();
        Instance._UIManager = new UIManager();
        Instance.EnemyList = new List<EnemyAbstractController>();      
    }

    void Start()
    {
        PlayerLife = 5;

        Observable.EveryUpdate()
             .Where(_ => PlayerLife == 0)
             .Subscribe(_ => Debug.Log("death"));
    }

    public static CharacterManager Instance
    {
        get
        {           
            return _instance;
        }

    }

    public void PlayerDamage(int damage)
    {
        if (PlayerLife > 0)
        {
            PlayerLife -= damage;
            Debug.Log(PlayerLife);
        }
    }

    public void AddEnemy(EnemyAbstractController enemy)
    {
        Instance.EnemyList.Add(enemy);       
    }

}
