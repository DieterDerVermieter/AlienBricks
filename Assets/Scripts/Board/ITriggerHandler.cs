using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    /// <summary>
    /// Implement to receive callbacks when a ball enters your trigger.
    /// </summary>
    public interface ITriggerHandler
    {
        /// <summary>
        /// Gets called by balls when they enter the trigger.
        /// </summary>
        /// <param name="ball">The ball that entered the trigger.</param>
        public void OnBallEnter(BallController ball);

        /// <summary>
        /// Gets called by balls every step they moved inside the trigger.
        /// </summary>
        /// <param name="ball">The ball that moved in the trigger.</param>
        public void OnBallStay(BallController ball);
    }
}
