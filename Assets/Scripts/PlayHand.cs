using UnityEngine;
using System.Collections;
using Leap.Unity;

namespace Leap.Unity
{
    public class PlayHand : IHandModel
    {
        [SerializeField]
        private GameObject HandObject;
   
        [SerializeField]
        private Chirality handedness;

        private Hand hand_;
        public override ModelType HandModelType
        {
            get
            {
                return ModelType.Graphics;
            }
        }

        public override Chirality Handedness
        {
            get
            {
                return handedness;
            }
            set { }
        }

        public override bool SupportsEditorPersistence()
        {
            return true;
        }

        public override Hand GetLeapHand()
        {
            return hand_;
        }

        public override void SetLeapHand(Hand hand)
        {
            hand_ = hand;
        }

        public override void UpdateHand()
        {
            ShowHandModel();   
        }

        protected void ShowHandModel()
        {
            Hand hand = GetLeapHand();
            HandObject.transform.position = hand.PalmPosition.ToVector3();
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
