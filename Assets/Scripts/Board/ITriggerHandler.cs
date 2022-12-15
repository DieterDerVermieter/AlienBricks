using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public interface ITriggerHandler
    {
        public void Trigger(BallController ball);
    }
}
