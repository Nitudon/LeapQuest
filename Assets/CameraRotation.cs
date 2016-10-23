using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    [SerializeField]
    private CanvasGroup canvas;

	void Start()
    {
        canvas.DOFade(0,2f);

        Observable.Timer(System.TimeSpan.FromSeconds(15f))
            .Subscribe(_ => SceneManager.LoadScene("Title"));

    }

    void Update()
    {
        transform.Rotate(new Vector3(0f,1f,0f));
    }

}
