using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// StepManager
/// 進行管理、ステップ数の記憶や、それに関わる命令を行う
/// </summary>
public class StepManager : MonoBehaviour {

    //instance
    private static StepManager _instance;

    //最初のステップから管理するため、最初に立ち上げ
    void Awake()
    {
        _instance = GameObject.FindGameObjectWithTag("StepManager").GetComponent<StepManager>();
        _instance.audioPlayer = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _instance.UICanvas = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _instance.FadeCanvas = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
        _instance.PauseBlur = GameObject.FindGameObjectWithTag("Player").GetComponent<Blur>();
        _instance.TransObjects = GameObject.Find("TransObjects");
        _instance.TransObjects.SetActive(false);
        _instance.battleStep = 0;
        _instance.PauseIndex = new IntReactiveProperty();
        _instance.PauseIndex.Value = 0;
        
        //スタート
        FadeCanvas.DOFade(0,3f)
        .OnComplete(() =>PlayerUIManager.Instance.OnPlayerWalk());

        //ポーズ関連

        PauseIndex
            .Where(_ => Time.timeScale == 0)
            .Subscribe(_ => UICanvas.OnPauseMenu(PauseIndex.Value % 3));

        Observable.EveryUpdate()
            .Where(_ => (Time.timeScale == 0) && (Input.GetKeyDown(KeyCode.Z)))
            .Subscribe(_ => OnPauseMenu((PauseMenu)(PauseIndex.Value % 3)));

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => OnPause());

        Observable.EveryUpdate()
            .Where(_ => (Time.timeScale == 0) && (Input.GetKeyDown(KeyCode.UpArrow) && PauseIndex.Value > 0))
            .Subscribe(_ => PauseIndex.Value--);

        Observable.EveryUpdate()
            .Where(_ => (Time.timeScale == 0) && (Input.GetKeyDown(KeyCode.DownArrow)))
            .Subscribe(_ => PauseIndex.Value++);

    }

    //get_instance
    public static StepManager Instance
    {
        get{ 
            return _instance;
        }
    }

    public int battleStep { get; private set; }//現在のバトルステップ
    private GameObject PauseObject;//ポーズトリガーオブジェクト
    private GameObject TransObjects;//ポーズ時に表示するオブジェクト
    private Blur PauseBlur;
    private AudioManager audioPlayer;//BGM管理
    private UIManager UICanvas;//バトルヘッダー
    private CanvasGroup FadeCanvas;//フェード用のキャンバス
    private IntReactiveProperty PauseIndex;
    public enum PauseMenu { pauseback,retry,end}
    //バトル終了、及び最初の移動
    public void OnWalk()
    {
        PlayerUIManager.Instance.OnPlayerWalk();
        audioPlayer.SoundChange(AudioManager.Music.walk);
    }

    //バトル終了処理
    public void OnBattleEnd()
    {
        UICanvas.OnBattleEnd(battleStep - 1);

        if (battleStep < 5)
        {
            OnWalk();
        }

        else if(battleStep == 5)
        {
            GameClearStep();
        }
    }

    //ゲームクリア処理
    public void GameClearStep()
    {
        FadeCanvas.DOFade(1, 3f)
        .OnComplete(() => SceneManager.LoadScene("GameClear"));
    }

    //ゲームオーバー処理
    public void GameOverStep()
    {
        FadeCanvas.DOFade(1, 3f)
        .OnComplete(() => SceneManager.LoadScene("GameOver"));
    }

    //ステップ更新とバトル通知
    public void OnBattle()
    {
        
        EnemyManager.Instance.BattleStart(battleStep);
        UICanvas.OnBattle(++battleStep);
        if (battleStep < 5)
        {
            audioPlayer.SoundChange(AudioManager.Music.battle);
        }

        else
        {
            audioPlayer.SoundChange(AudioManager.Music.boss);
        }
    }

    private void OnPauseMenu(PauseMenu index)
    {
        switch (index)
        {
            case PauseMenu.pauseback:
                OnPause();
                break;

            case PauseMenu.retry:
                Time.timeScale = 1;
                SceneManager.LoadScene("Main");
                break;

            case PauseMenu.end:
                Application.Quit();
                break;
        }
    }

    //ポーズ処理
    public void OnPause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            UICanvas.OnPause(true);
            TransObjects.SetActive(false);
            PauseBlur.enabled = false;
            
        }
        else
        {
            Time.timeScale = 0;
            UICanvas.OnPause(false);
            TransObjects.SetActive(true);
            PauseBlur.enabled = true;
        }

    }

}
