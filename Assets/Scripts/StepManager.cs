using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
        _instance.battleStep = 0;
        
        //スタート
        FadeCanvas.DOFade(0,3f)
        .OnComplete(() =>PlayerUIManager.Instance.OnPlayerWalk());
    }

    //get_instance
    public static StepManager Instance
    {
        get{ 
            return _instance;
        }
    }

    public int battleStep { get; private set; }//現在のバトルステップ
    private AudioManager audioPlayer;//BGM管理
    private UIManager UICanvas;//バトルヘッダー
    private CanvasGroup FadeCanvas;
    //バトル終了、及び最初の移動
    public void OnWalk()
    {
        PlayerUIManager.Instance.OnPlayerWalk();
        audioPlayer.SoundChange(AudioManager.Music.walk);
    }

    public void OnBattleEnd()
    {
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
        UICanvas.OnBattle(++battleStep);
        audioPlayer.SoundChange(AudioManager.Music.battle);
    }

    

}
