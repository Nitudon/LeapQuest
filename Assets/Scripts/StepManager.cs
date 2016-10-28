using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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

     private IObservable<UnityEngine.Collider> OnTriggerEnterObservable(GameObject obj)
    {
        return obj.OnTriggerEnterAsObservable()
            .Where(x => x.gameObject == PauseObject);
    }

    private IObservable<UnityEngine.Collider> OnTriggerExitObservable(GameObject obj)
    {
        return obj.OnTriggerExitAsObservable()
           .Where(x => x.gameObject == PauseObject);
    }

    private IObservable<long> OnTriggerHoldObservable(GameObject obj)
    {
        return
            OnTriggerEnterObservable(obj)
            .SelectMany(_ => Observable.Timer(System.TimeSpan.FromSeconds(2)))
            .TakeUntil(OnTriggerExitObservable(obj))
            .RepeatUntilDestroy(this);
    }

    //instance
    private static StepManager _instance;

    //最初のステップから管理するため、最初に立ち上げ
    void Awake()
    {
        _instance = GameObject.FindGameObjectWithTag("StepManager").GetComponent<StepManager>();
        _instance.audioPlayer = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _instance.UICanvas = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _instance.PauseObject = GameObject.Find("StartObject");
        _instance.PauseTint = UICanvas.transform.FindChild("PauseTint").gameObject;
        _instance.FadeCanvas = GameObject.FindGameObjectWithTag("Fade").GetComponent<CanvasGroup>();
        _instance.BattleComments = GameObject.Find("TextPanel").GetComponentsInChildren<Text>(true);
        _instance.Handpalm = GameObject.FindGameObjectsWithTag("Hand");
        _instance.battleStep = 0;
        
        //スタート
        FadeCanvas.DOFade(0,3f)
        .OnComplete(() =>PlayerUIManager.Instance.OnPlayerWalk());

        foreach (GameObject obj in Handpalm)
        {
            OnTriggerHoldObservable(obj)
            .Subscribe(_ => OnPause(obj));
        }
    }

    //get_instance
    public static StepManager Instance
    {
        get{ 
            return _instance;
        }
    }

    public int battleStep { get; private set; }//現在のバトルステップ
    private GameObject[] Handpalm;
    private GameObject PauseObject;
    private GameObject PauseTint;
    private AudioManager audioPlayer;//BGM管理
    private UIManager UICanvas;//バトルヘッダー
    private CanvasGroup FadeCanvas;
    private Text[] BattleComments;
    //バトル終了、及び最初の移動
    public void OnWalk()
    {
        PlayerUIManager.Instance.OnPlayerWalk();
        audioPlayer.SoundChange(AudioManager.Music.walk);
    }

    public void OnBattleEnd()
    {
        BattleComments[battleStep - 1].gameObject.SetActive(false);

        if (battleStep < 5)
        {
            OnWalk();
        }

        else if(battleStep == 5)
        {
            GameClearStep();
        }
    }

    public void GameClearStep()
    {
        FadeCanvas.DOFade(1, 3f)
        .OnComplete(() => SceneManager.LoadScene("GameClear"));
    }

    public void GameOverStep()
    {
        FadeCanvas.DOFade(1, 3f)
        .OnComplete(() => SceneManager.LoadScene("GameOver"));
    }

    //ステップ更新とバトル通知
    public void OnBattle()
    {
        EnemyManager.Instance.BattleStart(battleStep);
        BattleComments[battleStep].gameObject.SetActive(true);
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

    private void OnPause(GameObject obj)
    {
        Debug.Log("a");

        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PauseTint.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            PauseTint.SetActive(true);
        }

        OnTriggerHoldObservable(obj)
            .Subscribe(_ => OnPause(obj));
    }

}
