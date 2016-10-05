using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class UIManager : MonoBehaviour,IBattleMessage {

    #region[UIParts]
    private Text BattleHeader;
    private Text BattleStep;
    #endregion
    
    public void OnBattle(int step)
    {
        BattleHeader.gameObject.SetActive(true);
        BattleStep.text = step.ToString();
    }

    // Use this for initialization
    void Start () {
        BattleHeader = transform.FindChild("BattleHeader").gameObject.GetComponent<Text>();
        BattleStep = BattleHeader.gameObject.transform.FindChild("BattleStep").gameObject.GetComponent<Text>();

        BattleHeader.gameObject.OnEnableAsObservable()
            .Delay(System.TimeSpan.FromSeconds(4f))
            .Subscribe(_ => BattleHeader.gameObject.SetActive(false));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
