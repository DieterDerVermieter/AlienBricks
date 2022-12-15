using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public interface IImpactHandler
    {
        public void HandleImpact(BallController ball);
    }
}
