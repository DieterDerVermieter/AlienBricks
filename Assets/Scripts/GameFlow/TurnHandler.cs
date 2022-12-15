using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace DieterDerVermieter
{
    public abstract class TurnHandler : MonoBehaviour
    {
        /// <summary>
        /// Indicates if this Handler is currently doing something in it's turn.
        /// </summary>
        public bool IsTurnActive { get; protected set; }


        /// <summary>
        /// Gets called by the <see cref="GameManager"/> when this Handlers turn starts.
        /// Use <see cref="IsTurnActive"/> to indicate when your turn is done.
        /// </summary>
        public abstract void StartTurn();
    }
}
