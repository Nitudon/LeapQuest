using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameOverObjectMover : MonoBehaviour {
    [SerializeField]
    private CanvasGroup canvas;
	// Use this for initialization
	void Start () {

        canvas.DOFade(1,9f);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(0.6f,2f));
        seq.Append(transform.DORotate(new Vector3(-68.01f,-150.79f,0f),1f));
	}
	
}
