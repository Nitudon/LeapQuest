using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class HandlePanel : MonoBehaviour {

    private Collider leftCollider;
    private Collider rightCollider;

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
        exaObjects[exaIndex].SetActive(true);
        exaPanels[exaIndex].SetActive(true);
    }

	// Use this for initialization
	void Start () {
        exaIndex = 0;

        this.OnTriggerEnterAsObservable()
            .Where(x => x.name == "Left")
            .Subscribe(_ => UIChangeLeft());

        this.OnTriggerEnterAsObservable()
            .Where(x => x.name == "Right")
            .Subscribe(_ => UIChangeRight());

    }
	
}
