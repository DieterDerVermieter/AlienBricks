using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DieterDerVermieter
{
    public class UIController : MonoBehaviour
    {
        [Header("Pause Panel")]
        [SerializeField] private GameObject m_pausePanel;
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_restartButton;
        [SerializeField] private Button m_titleButton;

        [Header("Game Panel")]
        [SerializeField] private Button m_pauseButton;
        [SerializeField] private TMP_Text m_comboText;
        [SerializeField] private TMP_Text m_scoreText;


        private void OnEnable()
        {
            m_resumeButton.onClick.AddListener(ResumeButtonOnClick);
            m_restartButton.onClick.AddListener(RestartButtonOnClick);
            m_titleButton.onClick.AddListener(TitleButtonOnClick);
            m_pauseButton.onClick.AddListener(PauseButtonOnClick);
        }

        private void OnDisable()
        {
            m_resumeButton.onClick.RemoveListener(ResumeButtonOnClick);
            m_restartButton.onClick.RemoveListener(RestartButtonOnClick);
            m_titleButton.onClick.RemoveListener(TitleButtonOnClick);
            m_pauseButton.onClick.RemoveListener(PauseButtonOnClick);
        }


        private void Update()
        {
            m_pausePanel.SetActive(GameManager.Instance.IsPaused);


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
            GameManager.Instance.Restart();
        }

        private void TitleButtonOnClick()
        {

        }

        private void PauseButtonOnClick()
        {
            GameManager.Instance.Pause();
        }
    }
}
