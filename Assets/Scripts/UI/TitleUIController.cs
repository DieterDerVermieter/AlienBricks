using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DieterDerVermieter
{
    public class TitleUIController : MonoBehaviour
    {
        [SerializeField] private int m_gameSceneIndex;

        [Header("Pause Panel")]
        [SerializeField] private Button m_startButton;
        [SerializeField] private Button m_settingsButton;
        [SerializeField] private Button m_quitButton;


        private void OnEnable()
        {
            m_startButton.onClick.AddListener(StartButtonOnClick);
            m_settingsButton.onClick.AddListener(SettingsButtonOnClick);
            m_quitButton.onClick.AddListener(QuitButtonOnClick);
        }

        private void OnDisable()
        {
            m_startButton.onClick.RemoveListener(StartButtonOnClick);
            m_settingsButton.onClick.RemoveListener(SettingsButtonOnClick);
            m_quitButton.onClick.RemoveListener(QuitButtonOnClick);
        }


        private void StartButtonOnClick()
        {
            SceneManager.LoadScene(m_gameSceneIndex);
        }

        private void SettingsButtonOnClick()
        {

        }

        private void QuitButtonOnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
