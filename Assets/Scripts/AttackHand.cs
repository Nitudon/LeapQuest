using UnityEngine;
using System.Collections;
using Leap.Unity;
using UniRx;
using System.Linq;

namespace Leap.Unity
{
    public class AttackHand : CapsuleHand
    {
        private readonly Vector3 SWORD_OFFSET_POSITION = new Vector3(0f,-0.01f,0.03f);
        private bool _isGrab = false;

        [SerializeField]
        private GameObject FireParticle;

        [SerializeField]
        private GameObject SwordObject;

        [SerializeField]
        private GameObject GunObject;

        public override void InitHand()
        {
            base.InitHand();
            SwordObject.SetActive(false);
        }

        public override void UpdateHand()
        {
            base.UpdateHand();
           // updateSword();
        }

        private bool isGrab()
        {
           if(GetLeapHand().GrabStrength > 0.98f)
            {
                return true;
            }
            return false;
        }

        private bool isGun()
        {
            if(GetLeapHand().Fingers
                               .Where(finger => (finger.Type == Finger.FingerType.TYPE_INDEX || finger.Type == Finger.FingerType.TYPE_THUMB || finger.Type == Finger.FingerType.TYPE_MIDDLE) && (finger.IsExtended))
                               .Count() ==2)
            {
                return true;
            }

            return false;
        }

        private void updateSword()
        {
            Vector3 palmPosition = GetLeapHand().PalmPosition.ToVector3();

            //SwordObject.transform.localEulerAngles = new Vector3(0f,0f,90f) +  90 * new Vector3(GetLeapHand().PalmNormal.ToVector3().y, 0f,GetLeapHand().PalmNormal.ToVector3().x);
            SwordObject.transform.position = palmPosition + SWORD_OFFSET_POSITION;
            SwordObject.SetActive(isGrab());
        }

        private void updateGun()
        {
            Vector3 palmPosition = GetLeapHand().PalmPosition.ToVector3();

            GunObject.transform.position = palmPosition + SWORD_OFFSET_POSITION;
            GunObject.SetActive(isGrab());
        }

        private void Fire()
        {

        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
