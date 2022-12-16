using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DieterDerVermieter
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }


        [SerializeField] private List<TurnHandler> m_turnHandlers;


        private int m_currentTurnHandlerIndex;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


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


        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Pause();
            }
        }


        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0;
        }

        public void Unpause()
        {
            IsPaused = false;
            Time.timeScale = 1;
        }


        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
