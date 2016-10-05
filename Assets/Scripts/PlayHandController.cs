/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2016.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;
using System.Collections.Generic;
using System.Linq;

namespace Leap.Unity
{
    /** A physics model for our rigid hand made out of various Unity Collider. */
    public class PlayHandController : RigidHand
    {
        #region[Private Parameter]
        private Controller controller;
        private bool oldIsGrab_;
        #endregion

        void Start()
        {
            controller = new Controller();
            oldIsGrab_ = false;
        }

        void Update()
        {
            if (oldIsGrab_ != isGrab() && hand_ != null)
            {
                if (isGrab())
                {
                    Debug.Log("Grab");
                }

                else
                {
                    Debug.Log("Re");
                }
            }
        }

        private bool isGrab()
        {

            bool isGrab_ = (controller.Frame().Hands[0].Fingers
                                .Where(finger => finger.IsExtended)
                                .Count() < 2);

            return isGrab_;
        }
    }
}
