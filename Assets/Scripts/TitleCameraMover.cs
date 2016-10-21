using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// タイトルシーンのカメラワーク
/// </summary>
public class TitleCameraMover : MonoBehaviour {

	void Start () {
        transform.DORotate(new Vector3(0f, 360f, 0f), 30f)
            .SetRelative()
            .SetLoops(100);

        var seq = DOTween.Sequence();
        seq.Append(transform.DOMoveZ(100f, 18f)).SetEase(Ease.Linear);
        seq.Append(transform.DOMoveX(-150f, 18f)).SetEase(Ease.Linear);
        seq.Append(transform.DOMoveZ(-100f, 18f)).SetEase(Ease.Linear);
        seq.Append(transform.DOMoveX(150f, 18f)).SetEase(Ease.Linear);
        seq.SetLoops(100);

    }
	
}
