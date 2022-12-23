using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    [System.Serializable]
    public class SoundEffectData
    {
        public const int MAX_PRIORITY = 127;


        public AudioClip Clip;

        [Range(0, MAX_PRIORITY)]
        public int Priority = 0;
    }
}
