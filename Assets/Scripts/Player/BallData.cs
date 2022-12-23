using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    [CreateAssetMenu]
    public class BallData : ScriptableObject
    {
        public string DisplayName = "Coold Ball";
        public Sprite DisplayIcon;

        public Sprite BallSprite;
        public float BallRadius = 0.2f;
        public int BallDamage = 1;

        public GameObject BallImpactEffectPrefab;
        public SoundEffectData BallImpactSound;

        public int UseCooldown = 0;
    }
}
