using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Leap;
using UniRx;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Leap.Unity
{
    [RequireComponent(typeof(HandPool))]
    public class TitleFadeController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvas;

        [SerializeField]
        private string fadeScene;

        private List<HandPool.ModelGroup> handmodels;
        private HandPool.ModelGroup handmodel;
        private AudioManager audioPlayer;
        private IHandModel leftModel;
        private IHandModel rightModel;

        private IObservable<long> DoubleGutsObservable()
        {
            return Observable.EveryUpdate()
                .Where(_ => isGuts(leftModel) && isGuts(rightModel));
        }

        private IObservable<long> DoubleGutsCancelObservable()
        {
            return Observable.EveryUpdate()
                .Where(_ => !(isGuts(leftModel) && isGuts(rightModel)));
        }

        private IObservable<long> StartGutsObservable()
        {
            return
                DoubleGutsObservable()
                .SelectMany(_ => Observable.Timer(System.TimeSpan.FromSeconds(1)))
                .TakeUntil(DoubleGutsCancelObservable())
                .RepeatUntilDestroy(this);
        }

        void Start()
        {
            canvas.DOFade(0, 3f);
            audioPlayer = transform.parent.GetComponent<AudioManager>();
            handmodels = GetComponent<HandPool>().ModelPool;
            handmodel = handmodels[0];
            leftModel = handmodel.LeftModel;
            rightModel = handmodel.RightModel;

            StartGutsObservable()
                .First()
                .Subscribe(_ => FadeScene());
        }

        private bool isGuts(IHandModel hand)
        {
            if (hand.GetLeapHand() != null)
                return hand.GetLeapHand().Fingers
                    .Where(finger => ((finger.Type != Finger.FingerType.TYPE_THUMB) && !finger.IsExtended) || ((finger.Type == Finger.FingerType.TYPE_THUMB) && finger.IsExtended))
                    .Count() == 5;

            else return false;
        }

        private void FadeScene()
        {
            audioPlayer.SoundEffect(AudioManager.SE.Trans);
            canvas.DOFade(1,3f)
                .OnComplete(() => SceneManager.LoadScene(fadeScene));
        }

    }
}
