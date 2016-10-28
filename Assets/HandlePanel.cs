using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class HandlePanel : MonoBehaviour {

    private GameObject leftCollider;
    private GameObject rightCollider;

    private int exaIndex;

    [SerializeField]
    private GameObject[] exaPanels;

    [SerializeField]
    private GameObject[] exaObjects;

    public enum Dir { left,right}

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
