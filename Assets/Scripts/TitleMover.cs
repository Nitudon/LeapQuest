using UnityEngine;
using System.Collections;
using UniRx;
using DG.Tweening;

/// <summary>
/// タイトル用に配置している敵をそれとなく動かす
/// </summary>

public class TitleMover : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        Observable.Timer(System.TimeSpan.FromSeconds(0),System.TimeSpan.FromSeconds(6))
            .Subscribe(_ => RandomMove())
            .AddTo(gameObject);
    }

    void RandomMove()
    {
        transform.DOJump(transform.position + new Vector3(Random.Range(-15, 15), 0f, Random.Range(-15, 15)), 0.6f, 3, 5f);
    }

}
