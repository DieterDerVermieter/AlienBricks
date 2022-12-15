using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public abstract class TurnHandler : MonoBehaviour
{
    public System.Action OnMoveDone;


    public abstract void MakeMove();
}
