using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

/// <summary>
/// キャラ説明パネルの操作
/// </summary>
/// 
public class HandlePanel : MonoBehaviour {

    private GameObject leftCollider;//左送りのボタン
    private GameObject rightCollider;//右送りのボタン

    private int exaIndex;//現在表示されている情報

    [SerializeField]
    private GameObject[] exaPanels;//説明パネル

    [SerializeField]
    private GameObject[] exaObjects;//説明している敵のオブジェクト

    public enum Dir { left,right}

    //右送り
    private void UIChangeRight()
    {
            exaObjects[exaIndex].SetActive(false);
            exaPanels[exaIndex].SetActive(false);
            exaIndex++;
            if (exaIndex == exaPanels.Length)
            {
                exaIndex = 0;
            }
        StartCoroutine(WaitSetActive());
            exaObjects[exaIndex].SetActive(true);
            exaPanels[exaIndex].SetActive(true);
       
    }

    //左送り
    private void UIChangeLeft()
    {
        exaObjects[exaIndex].SetActive(false);
        exaPanels[exaIndex].SetActive(false);
        exaIndex--;
        if (exaIndex < 0)
        {
            exaIndex = exaPanels.Length - 1;
        }
        StartCoroutine(WaitSetActive());
        exaObjects[exaIndex].SetActive(true);
        exaPanels[exaIndex].SetActive(true);
    }

    //連続処理対策
    private IEnumerator WaitSetActive()
    {
        yield return new WaitForSeconds(0.5f);

        yield break;
    }

	// Use this for initialization
	void Start () {
        exaIndex = 0;

        leftCollider = GameObject.Find("Left");
        rightCollider = GameObject.Find("Right");

        leftCollider.OnTriggerEnterAsObservable()
            .Where(x => x.tag == "Hand")
            .Subscribe(_ => UIChangeLeft());

        rightCollider.OnTriggerEnterAsObservable()
            .Where(x => x.tag == "Hand")
            .Subscribe(_ => UIChangeRight());

    }
	
}
