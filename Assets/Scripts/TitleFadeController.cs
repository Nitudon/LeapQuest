using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Leap;
using UniRx;
using UniRx.Triggers;
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
        private CanvasGroup logoCanvas;

        [SerializeField]
        private string fadeScene;

        [SerializeField]
        private GameObject StartObject;

        [SerializeField]
        private GameObject EndObject;

        private List<HandPool.ModelGroup> handmodels;
        private HandPool.ModelGroup handmodel;
        private AudioManager audioPlayer;
        private IHandModel leftModel;
        private IHandModel rightModel;
        private static bool gameStart;

        private IObservable<UnityEngine.Collider> OnTriggerEnterObservable(GameObject obj)
        {
            return obj.OnTriggerEnterAsObservable();
                //.Where(x => x.gameObject == StartObject);
        }

        private IObservable<UnityEngine.Collider> OnTriggerExitObservable(GameObject obj)
        {
            return obj.OnTriggerExitAsObservable()
                .Where(x => x.gameObject == StartObject);
        }

        private IObservable<long> OnTriggerHoldObservable(GameObject obj)
        {
            return
                OnTriggerEnterObservable(obj)
                .SelectMany(_ => Observable.Timer(System.TimeSpan.FromSeconds(2)))
                .TakeUntil(OnTriggerExitObservable(obj))
                .RepeatUntilDestroy(this);
        }

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
            if(Application.loadedLevelName == "Title" && !gameStart)
            {
                gameStart = true;
                Sequence seq = DOTween.Sequence();
                seq.Append(logoCanvas.DOFade(1, 3f));
                seq.Append(logoCanvas.DOFade(0, 3f));
                seq.OnComplete(() => canvas.DOFade(0, 3f));
            }
            else
            {
                canvas.DOFade(0, 3f);
            }

            audioPlayer = transform.parent.GetComponent<AudioManager>();
            handmodels = GetComponent<HandPool>().ModelPool;
            handmodel = handmodels[0];
            leftModel = handmodel.LeftModel;
            rightModel = handmodel.RightModel;

            OnTriggerHoldObservable(StartObject)
                .Subscribe(_ => Debug.Log("aa"));

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
