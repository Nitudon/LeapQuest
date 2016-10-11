using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class StepManager : MonoBehaviour {

    private static StepManager _instance;

    void Awake()
    {
        _instance = GameObject.FindGameObjectWithTag("StepManager").GetComponent<StepManager>();
        _instance.audioPlayer = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _instance.UICanvas = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _instance.battleStep = 0;

        PlayerUIManager.Instance.OnPlayerWalk();
    }

    public static StepManager Instance
    {
        get{ 
            return _instance;
        }
    }
    private int battleStep;
    private AudioManager audioPlayer;
    private UIManager UICanvas;


    public void OnBattle()
    {
        EnemyManager.Instance.BattleStart(battleStep);
        UICanvas.OnBattle(++battleStep);
    }

    

}
