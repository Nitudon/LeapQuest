using UnityEngine;
using System.Collections;
using Leap.Unity;
using System.Linq;
using System.Collections.Generic;

namespace Leap.Unity
{
    [RequireComponent(typeof(LeapServiceProvider))]

    public class HandManager : MonoBehaviour
    {
        private Controller controller;

        #region[Private Parameter]
        private bool oldGrab;
        private List<Hand> Hands;
        #endregion

        // Use this for initialization
        void Start()
        {
            oldGrab = false;
            controller = GetComponent<LeapServiceProvider>().GetLeapController();
        }

        private bool isGrab
        {
            get
            {
                return Hands
                        .Where(hand => hand.IsRight && hand.GrabStrength > 0.9)
                        .Count() == 1;
            }
        }

        private bool isGun
        {
            get
            {
                return Hands
                        .Where(hand => hand.IsRight &&
                                hand.Fingers.Where(finger => (finger.Type == Finger.FingerType.TYPE_INDEX || finger.Type == Finger.FingerType.TYPE_MIDDLE || finger.Type == Finger.FingerType.TYPE_THUMB) && (finger.IsExtended))
                                            .Count() >= 2)
                        .Count() == 1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            Hands = controller.Frame().Hands;
        }
    }
}
