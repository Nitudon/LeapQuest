using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class UIManager : MonoBehaviour,IBattleMessage {

    private readonly float[] PAUSE_ARROW_POSITION = 
    {
        25f,
        -220f,
        -445f
    };

    private readonly Vector3[] BATTLEHEADER_POSITION = new Vector3[]
   {
       new Vector3(-63.8f,65.4f,0f),
       new Vector3(-63.8f,2565.4f,0f)
    };
    private const float TEXTPANEL_FADE_IN = -335f;//バトルコメントパネルの位置
    private const float TEXTPANEL_FADE_OUT = -575f;//バトルコメントパネルの待機位置

    #region[UIParts]
    private GameObject PauseTint;//ポーズ表示UI
    private Text BattleHeader;//バトル開始時のバトル数表示
    private Text BattleStep;//バトル数
    private GameObject PlayerGaze;//プレイヤーのHPゲージ
    private GameObject PauseArrow;//ポーズメニューの矢印
    private GameObject BattleCommentPanel;//バトルコメントを表示するパネル
    private Text[] BattleComments;//バトルコメントのオブジェクト
    #endregion

    //バトル開始処理
    public void OnBattle(int step)
    {
        StartCoroutine(BattleComment(step-1));
        BattleComments[step-1].gameObject.SetActive(true);
        BattleHeader.gameObject.SetActive(true);
        BattleStep.text = step.ToString();
    }

    //バトル終了処理
    public void OnBattleEnd(int step)
    {
        BattleComments[step].gameObject.SetActive(false);
    }

    //ポーズ処理
    public void OnPause(bool pause)
    {
        BattleHeader.transform.localPosition =  (pause) ?BATTLEHEADER_POSITION[0] : BATTLEHEADER_POSITION[1];
        PlayerGaze.SetActive(pause);
        BattleCommentPanel.SetActive(pause);
        PauseTint.SetActive(!pause);
    }

    //ポーズ矢印処理
    public void OnPauseMenu(int index)
    {
        PauseArrow.transform.localPosition = new Vector3(PauseArrow.transform.localPosition.x,PAUSE_ARROW_POSITION[index],0);
    }

    //バトルコメントの表示コルーチン
    private IEnumerator BattleComment(int step)
    {
        BattleComments[step].gameObject.SetActive(true);
        BattleCommentPanel.transform.DOLocalMoveY(TEXTPANEL_FADE_IN, 1f);
        yield return new WaitForSeconds(10f);
        BattleCommentPanel.transform.DOLocalMoveY(TEXTPANEL_FADE_OUT, 3f);
        yield break;

    }

    // Use this for initialization
    void Start () {
        BattleHeader = transform.FindChild("BattleHeader").gameObject.GetComponent<Text>();
        BattleStep = BattleHeader.gameObject.transform.FindChild("BattleStep").gameObject.GetComponent<Text>();
        BattleCommentPanel = GameObject.Find("TextPanel");
        BattleComments = BattleCommentPanel.GetComponentsInChildren<Text>(true);
        PauseTint = transform.FindChild("PauseTint").gameObject;
        PlayerGaze = transform.FindChild("PlayerUI").gameObject;
        PauseArrow = PauseTint.transform.FindChild("PauseArrow").gameObject;

        BattleHeader.gameObject.OnEnableAsObservable()
            .Delay(System.TimeSpan.FromSeconds(4f))
            .Subscribe(_ => BattleHeader.gameObject.SetActive(false));

	}
	
}
