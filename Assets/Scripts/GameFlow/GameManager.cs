using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DieterDerVermieter
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<TurnHandler> m_turnHandlers;


        private int m_currentTurnHandlerIndex;


        private void Start()
        {
            m_turnHandlers[0].StartTurn();
        }


        private void Update()
        {
            if (!m_turnHandlers[m_currentTurnHandlerIndex].IsTurnActive)
            {
                m_currentTurnHandlerIndex = (m_currentTurnHandlerIndex + 1) % m_turnHandlers.Count;
                m_turnHandlers[m_currentTurnHandlerIndex].StartTurn();
            }
        }
    }
}
