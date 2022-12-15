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
            GameValues.Level = 1;
            GameValues.BallCount = 1;
            GameValues.Score = 0;

            m_turnHandlers[0].StartTurn();
        }


        private void Update()
        {
            if (!m_turnHandlers[m_currentTurnHandlerIndex].IsTurnActive)
            {
                m_currentTurnHandlerIndex++;
                if(m_currentTurnHandlerIndex >= m_turnHandlers.Count)
                {
                    m_currentTurnHandlerIndex = 0;
                    GameValues.Level++;
                }

                m_turnHandlers[m_currentTurnHandlerIndex].StartTurn();
            }
        }
    }
}
