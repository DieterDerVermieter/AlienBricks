using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    /// <summary>
    /// Implement to receive callbacks when a ball is hitting your collider.
    /// </summary>
    public interface IImpactHandler
    {
        /// <summary>
        /// Gets called by balls when they hit the collider.
        /// </summary>
        /// <param name="ball">The ball that hit the collider.</param>
        public void HandleImpact(BallController ball);
    }
}
