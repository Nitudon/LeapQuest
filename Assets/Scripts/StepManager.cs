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
        _instance.BattleCommentPanel = GameObject.Find("TextPanel");
        _instance.BattleComments = BattleCommentPanel.GetComponentsInChildren<Text>(true);
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

    private const float TEXTPANEL_FADE_IN = -335f;//バトルコメントパネルの位置
    private const float TEXTPANEL_FADE_OUT = -575f;//バトルコメントパネルの待機位置

    public int battleStep { get; private set; }//現在のバトルステップ
    private GameObject[] Handpalm;//手のオブジェクト
    private GameObject PauseObject;//ポーズ時に表示するオブジェクト
    private GameObject PauseTint;//ポーズ表示UI
    private AudioManager audioPlayer;//BGM管理
    private UIManager UICanvas;//バトルヘッダー
    private CanvasGroup FadeCanvas;//フェード用のキャンバス
    private GameObject BattleCommentPanel;//バトルコメントを表示するパネル
    private Text[] BattleComments;//バトルコメントのオブジェクト

    //バトル終了、及び最初の移動
    public void OnWalk()
    {
        PlayerUIManager.Instance.OnPlayerWalk();
        audioPlayer.SoundChange(AudioManager.Music.walk);
    }

    //バトル終了処理
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
        StartCoroutine(BattleComment());
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

    //バトルコメントの表示コルーチン
    private IEnumerator BattleComment()
    {
        BattleComments[battleStep].gameObject.SetActive(true);
        BattleCommentPanel.transform.DOLocalMoveY(TEXTPANEL_FADE_IN,1f);
        yield return new WaitForSeconds(10f);
        BattleCommentPanel.transform.DOLocalMoveY(TEXTPANEL_FADE_OUT, 3f);
        yield break;

    }

    //ポーズ処理
    private void OnPause(GameObject obj)
    {

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
