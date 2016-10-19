using UnityEngine;
using System.Collections;

/// <summary>
/// BoxHand：BoxColliderによるRigitHandに、手オブジェクトを追加したもの
/// 手オブジェクトと、その付属品の処理を行う
/// </summary>
/// 
namespace Leap.Unity {
    public class BoxHand : SkeletalHand {

        public float filtering = 0.5f;

        /* 手と付属オブジェクト**/
        #region[HandRigits]
        private Rigidbody shieldBody;
        private Rigidbody palmBody;
        #endregion

        /* 内部パラメータ**/
        #region[PrivateParemeter]
        private readonly Vector3 shieldOffsetPosition = new Vector3(0f,0f,0.2f);
        #endregion

        /* 手の認知処理、盾オブジェクトのアクティブ化**/
        public override void BeginHand()
        {
            //ShieldObject.gameObject.SetActive(true);
            base.BeginHand();
        }
        /* 手の場外処理、盾オブジェクトの非アクティブ化**/
        public override void FinishHand()
        {
            base.FinishHand();
        }

        protected void Start()
        {
            palmBody = palm.GetComponent<Rigidbody>();
        }

        public override bool SupportsEditorPersistence()
        {
            return true;
        }

        public override ModelType HandModelType
        {
            get
            {
                return ModelType.Physics;
            }
        }

        public override void UpdateHand()
        {   

            if (palm != null)
            {
                /* 盾オブジェクト処理**/
                if(shieldBody)
                {
                    /* 握っている場合はアクティブに、位置を追従**/
                    if (hand_.GrabStrength < 0.1f)
                    {
                        shieldBody.MovePosition(GetPalmCenter() + shieldOffsetPosition);
                       // ShieldObject.gameObject.SetActive(true);
                    }
                }
                /* 手オブジェクト処理、セットされている場合追従させる**/
                if (palmBody)
                {
                    palmBody.MovePosition(GetPalmCenter());
                    palmBody.MoveRotation(GetPalmRotation());
                }
                else
                {
                    palm.position = GetPalmCenter();
                    palm.rotation = GetPalmRotation();
                }
            }

         }
      }
}