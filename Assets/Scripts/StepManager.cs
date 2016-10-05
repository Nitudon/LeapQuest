using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public class StepManager : MonoBehaviour {

    private struct TimeStep
    {
        public string trigger;
        public float time;

        public TimeStep(string _trigger,float _time)
        {
            trigger = _trigger;
            time = _time;
        }
    }

    [SerializeField]
    private TimeSchedule timeschedule;

    public IntReactiveProperty BattleStep;

    #region[Client]
    private PlayerMover Player;
    private TesGenerater EnemyGenerator;
    private UIManager UICanvas;
    #endregion

    #region[PrivateParameter]
    private int MoveStep;    
    private static List<TimeStep> TimeSchedule;
    private bool IsBattle;
    #endregion 

    // Use this for initialization
    void Start () {
        UICanvas = GameObject.Find("UICanvas").GetComponent<UIManager>();
        TimeSchedule = new List<TimeStep>();
        Player = GameObject.Find("Camera").GetComponent<PlayerMover>();
        IsBattle = false;
        EnemyGenerator = transform.FindChild("EnemyGenerator").gameObject.GetComponent<TesGenerater>();
        MoveStep = 0;

        for(int i = 0; i < timeschedule.times.Length; i++)
        {
            TimeSchedule.Add(new TimeStep(timeschedule.triggers[i],timeschedule.times[i]));
        }

        for (int i = 0; i < TimeSchedule.Count; i++)
        {
            Observable.Timer(System.TimeSpan.FromSeconds(TimeSchedule[i].time))
                .Subscribe(_ =>Step())
                    .AddTo(gameObject);
        }

        BattleStep
            .Where(_ => BattleStep.Value > 0)
            .Subscribe(_ => OnBattle());
    }

    private void OnBattle()
    {
        EnemyGenerator.OnBattle(BattleStep.Value);
        UICanvas.OnBattle(BattleStep.Value);
    }

    private void Step() { 

        if (TimeSchedule[MoveStep].trigger == "Stop")
        {
            EnemyGenerator.gameObject.SetActive(true);
            BattleStep.Value++;
        }

        else
        {
            EnemyGenerator.gameObject.SetActive(false);
            GameObject[] Enemys = GameObject.FindGameObjectsWithTag("Enemy");
            if (Enemys.Length > 0)
            {
                foreach (GameObject enemy in Enemys)
                {
                    var controller = enemy.GetComponent<EnemyAbstractController>();
                    controller.EndTurn();
                }
            }
        }
        Player.OnPlayerMove(TimeSchedule[MoveStep++].trigger);
    }

}
