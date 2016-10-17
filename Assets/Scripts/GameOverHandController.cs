using UnityEngine;
using UniRx;
using Leap;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Leap.Unity
{
    [RequireComponent(typeof(HandPool))]
    public class GameOverHandController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvas;

        [SerializeField]
        private GameObject Object;

        private List<HandPool.ModelGroup> handmodels;
        private HandPool.ModelGroup handmodel;
        private IHandModel leftModel;
        private IHandModel rightModel;

        private IObservable<long> GutsObservable(IHandModel hand)
        {
            return Observable.EveryUpdate()
                .Where(_ => isGuts(hand));
        }

        private IObservable<long> GutsCancelObservable(IHandModel hand)
        {
            return Observable.EveryUpdate()
                .Where(_ => !(isGuts(hand)));
        }

        private IObservable<long> StartGutsObservable(IHandModel hand)
        {
            return
                GutsObservable(hand)
                .SelectMany(_ => Observable.Timer(System.TimeSpan.FromSeconds(1)))
                .TakeUntil(GutsCancelObservable(hand))
                .RepeatUntilDestroy(gameObject);
        }

        void Start()
        {

            canvas.DOFade(1, 9f);

            Sequence seq = DOTween.Sequence();
            seq.Append(Object.transform.DOMoveY(0.6f, 2f));
            seq.Append(Object.transform.DORotate(new Vector3(-68.01f, -150.79f, 0f), 1f));

            handmodels = GetComponent<HandPool>().ModelPool;
            handmodel = handmodels[0];
            leftModel = handmodel.LeftModel;
            rightModel = handmodel.RightModel;

            StartGutsObservable(leftModel)
                .Subscribe(_ => FadeScene());

            StartGutsObservable(rightModel)
                .Subscribe(_ => GameEnd());
        }

        private bool isGuts(IHandModel hand)
        {
            if (hand != null)
                return hand.GetLeapHand().Fingers
                    .Where(finger => ((finger.Type != Finger.FingerType.TYPE_THUMB) && !finger.IsExtended) || ((finger.Type == Finger.FingerType.TYPE_THUMB) && finger.IsExtended))
                    .Count() == 5;

            else return false;
        }

        private void GameEnd()
        {
            canvas.DOFade(0, 7f)
               .OnComplete(() => Application.Quit());
        }

        private void FadeScene()
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(Object.transform.DORotate(new Vector3(0f, -150.79f, 0f), 1f));

            canvas.DOFade(0, 7f)
                .OnComplete(() => SceneManager.LoadScene("Main"));
        }

    }
}
