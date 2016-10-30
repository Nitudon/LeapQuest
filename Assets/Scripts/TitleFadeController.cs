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
        #region[SerializeObjects]
        [SerializeField]
        private CanvasGroup canvas;//フェード用キャンバス

        [SerializeField]
        private CanvasGroup logoCanvas;//タイトル用にとみくロゴキャンバス

        [SerializeField]
        private string fadeScene;//遷移させるシーン先

        [SerializeField]
        private GameObject StartObject;//遷移決定オブジェクト

        [SerializeField]
        private GameObject EndObject;//ゲーム終了決定オブジェクト
        #endregion

        private List<HandPool.ModelGroup> handmodels;//手のモデル集合
        private HandPool.ModelGroup handmodel;//手のモデル
        private AudioManager audioPlayer;//オーディオ
        private IHandModel leftModel;//左手オブジェクト
        private IHandModel rightModel;//右手オブジェクト

        //指定オブジェクトのTriggerEnter監視
        private IObservable<UnityEngine.Collider> OnTriggerEnterObservable(GameObject obj)
        {
            return obj.OnTriggerEnterAsObservable()
            .Where(x => x.tag == "Hand");
        }

        //指定オブジェクトのTriggerExit監視
        private IObservable<UnityEngine.Collider> OnTriggerExitObservable(GameObject obj)
        {
            return obj.OnTriggerExitAsObservable()
               .Where(x => x.tag == "Hand");
        }

        //指定オブジェクトのTriggerについて、一定時間stayしているかどうかの監視
        private IObservable<long> OnTriggerHoldObservable(GameObject obj)
        {
            return
                OnTriggerEnterObservable(obj)//Enter監視
                .SelectMany(_ => Observable.Timer(System.TimeSpan.FromSeconds(2)))//一定時間の監視
                .TakeUntil(OnTriggerExitObservable(obj))//Exit監視でHoldしているかどうかの判定
                .RepeatUntilDestroy(this)//ゲーム中に複数回の判定を行う
                .First();
        }

        //両手オブジェクトのガッツポーズ監視
        private IObservable<long> DoubleGutsObservable()
        {
            return Observable.EveryUpdate()
                .Where(_ => isGuts(leftModel) && isGuts(rightModel));
        }

        //両手オブジェクトのガッツポーズ解除監視
        private IObservable<long> DoubleGutsCancelObservable()
        {
            return Observable.EveryUpdate()
                .Where(_ => !(isGuts(leftModel) && isGuts(rightModel)));
        }

        //両手オブジェクトのガッツポーズの一定時間保持しているかの監視
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
            //イニシャライズと遷移設定
            audioPlayer = transform.parent.GetComponent<AudioManager>();
            handmodels = GetComponent<HandPool>().ModelPool;
            handmodel = handmodels[0];
            leftModel = handmodel.LeftModel;
            rightModel = handmodel.RightModel;

            //タイトルシーンのみにとみくロゴをスプラッシュさせるため特殊処理
            if(Application.loadedLevelName == "Title")
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(logoCanvas.DOFade(1, 3f));
                seq.Append(logoCanvas.DOFade(0, 3f));
                seq.OnComplete(() => {
                    canvas.DOFade(0, 1f);
                    StartObject.SetActive(true);
                    EndObject.SetActive(true);
                    audioPlayer.OnPlay();
                });
            }
            else
            {
                canvas.DOFade(0, 3f);
            }
                   
                OnTriggerHoldObservable(StartObject)
                    .First()
                    .Subscribe(_ => FadeScene());

                OnTriggerHoldObservable(EndObject)
                    .First()
                    .Subscribe(_ => Application.Quit());
                   
        }

        //ガッツポーズの判定
        private bool isGuts(IHandModel hand)
        {
            if (hand.GetLeapHand() != null)
                return hand.GetLeapHand().Fingers
                    .Where(finger => ((finger.Type != Finger.FingerType.TYPE_THUMB) && !finger.IsExtended) || ((finger.Type == Finger.FingerType.TYPE_THUMB) && finger.IsExtended))
                    .Count() == 5;

            else return false;
        }

        //シーン遷移処理
        private void FadeScene()
        {
            audioPlayer.SoundEffect(AudioManager.SE.Trans);
            canvas.DOFade(1,3f)
                .OnComplete(() => SceneManager.LoadScene(fadeScene));
        }

    }
}
