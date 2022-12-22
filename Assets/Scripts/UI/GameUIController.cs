using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DieterDerVermieter
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Game Panel")]
        [SerializeField] private Button m_pauseButton;
        [SerializeField] private TMP_Text m_comboText;
        [SerializeField] private TMP_Text m_scoreText;

        [Header("Pause Panel")]
        [SerializeField] private GameObject m_pausePanel;
        [SerializeField] private Button m_pauseResumeButton;
        [SerializeField] private Button m_pauseRestartButton;
        [SerializeField] private Button m_pauseSettingsButton;
        [SerializeField] private Button m_pauseTitleButton;

        [Header("GameOver Panel")]
        [SerializeField] private GameObject m_gameOverPanel;
        [SerializeField] private Button m_gameOverRestartButton;
        [SerializeField] private Button m_gameOverTitleButton;


        private void OnEnable()
        {
            m_pauseButton.onClick.AddListener(PauseButtonOnClick);

            m_pauseResumeButton.onClick.AddListener(ResumeButtonOnClick);
            m_pauseRestartButton.onClick.AddListener(RestartButtonOnClick);
            m_pauseSettingsButton.onClick.AddListener(SettingsButtonOnClick);
            m_pauseTitleButton.onClick.AddListener(TitleButtonOnClick);

            m_gameOverRestartButton.onClick.AddListener(RestartButtonOnClick);
            m_gameOverTitleButton.onClick.AddListener(TitleButtonOnClick);
        }

        private void OnDisable()
        {
            m_pauseButton.onClick.RemoveListener(PauseButtonOnClick);

            m_gameOverRestartButton.onClick.RemoveListener(ResumeButtonOnClick);
            m_pauseRestartButton.onClick.RemoveListener(RestartButtonOnClick);
            m_pauseSettingsButton.onClick.RemoveListener(SettingsButtonOnClick);
            m_pauseTitleButton.onClick.RemoveListener(TitleButtonOnClick);

            m_gameOverRestartButton.onClick.RemoveListener(RestartButtonOnClick);
            m_gameOverTitleButton.onClick.RemoveListener(TitleButtonOnClick);
        }


        private void Update()
        {
            m_pausePanel.SetActive(GameManager.Instance.IsPaused);
            m_gameOverPanel.SetActive(GameManager.Instance.IsGameOver);

            if (GameValues.Combo <= 0) m_comboText.text = "";
            else m_comboText.text = $"{GameValues.Combo}x";

            m_scoreText.text = GameValues.Score.ToString("#,##");
        }


        private void ResumeButtonOnClick()
        {
            GameManager.Instance.Unpause();
        }

        private void RestartButtonOnClick()
        {
            GameManager.Instance.RestartGame();
        }

        private void TitleButtonOnClick()
        {
            GameManager.Instance.TitleScreen();
        }

        private void PauseButtonOnClick()
        {
            GameManager.Instance.Pause();
        }

        private void SettingsButtonOnClick()
        {
            
        }
    }
}
