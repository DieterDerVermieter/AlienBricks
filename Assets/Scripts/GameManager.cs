using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<TurnHandler> m_turnHandlers;


    int m_currentTurnHandlerIndex;


    private void Start()
    {
        foreach (var turnHandler in m_turnHandlers)
        {
            turnHandler.OnMoveDone += OnMoveDoneHandler;
        }

        MakeNextMove();
    }


    private void MakeNextMove()
    {
        m_turnHandlers[m_currentTurnHandlerIndex].MakeMove();
        m_currentTurnHandlerIndex = (m_currentTurnHandlerIndex + 1) % m_turnHandlers.Count;
    }


    private void OnMoveDoneHandler()
    {
        MakeNextMove();
    }
}
