using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class PlayerUIManager : MonoBehaviour {

    /// <summary>
    /// PlayerManagerクラス
    /// UI、値管理を行う
    /// </summary>
    public Player _player;
    private static PlayerUIManager _instance;
    private PlayerMover _playerMover;
    private Image _lifeBar;
    private Text _lifeText;
    
    //Singleton
    void Awake()
    {
        if(this != Instance)
        {
            Destroy(this);
            Debug.Log("There are two Singleton objects:" + gameObject.name.ToString() );
        }
    }

    //イニシャライズ
    public void Init()
    {
        _player = new Player();
        _lifeBar = GameObject.FindGameObjectWithTag("LifeBar").GetComponent<Image>();
        _lifeText = GameObject.FindGameObjectWithTag("LifeText").GetComponent<Text>();
        _playerMover = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMover>();
    }
      
    //get_instance
    public static PlayerUIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                //Managerオブジェクト検索                
                _instance = GameObject.FindGameObjectWithTag("PlayerUIManager").GetComponent<PlayerUIManager>();
                //イニシャライズ
                _instance.Init();
                //ライフ値変動Observer,UI更新
                _instance._player._life
                    .Skip(1)//初期化の検知をスキップ
                    .TakeWhile(x => x >= 0)//正の値までを検知
                    .Subscribe(x => _instance.UIChangeWithLife(x));//UI更新

                _instance._player._life
                    .Where(x => x <= 0)
                    .Subscribe(_ => _instance.GameOver());
            }
            return _instance;
        }
    }

    private void GameOver()
    {
        StepManager.Instance.GameOverStep();
    }

    //描画
    private void UIChangeWithLife(float life)
    {
        _lifeBar.fillAmount = life / Instance._player.START_LIFE;
        _lifeText.text = life.ToString() + "/" + Instance._player.START_LIFE.ToString();
        _playerMover.OnPlayerDamage();//ダメージエフェクト
    }

    //ライフポイント影響
    public void LifeAffect(int point)
    {
        if (Instance._player._life.Value - point >= 0)
        {
            Instance._player.LifeAffect(point);
        }
        else
        {
            Instance._player._life.Value = 0;
        }
    }

    public void OnPlayerWalk()
    {
        _playerMover.OnPlayerWalk();
    }

}
