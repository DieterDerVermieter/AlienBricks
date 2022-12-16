using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DieterDerVermieter
{
    [RequireComponent(typeof(Button))]
    public class BallSelectionItem : MonoBehaviour
    {
        [SerializeField] private GameObject m_selectionFrame;
        [SerializeField] private GameObject m_onCooldownOverlay;
        [SerializeField] private TMP_Text m_cooldownText;
        [SerializeField] private Image m_iconImage;

        private Button m_button;

        private PlayerController m_playerController;

        private bool m_skipCountdown;


        public BallData Data { get; private set; }

        public int Cooldown { get; private set; }


        private void Awake()
        {
            m_button = GetComponent<Button>();
        }


        private void OnEnable()
        {
            m_button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            m_button.onClick.RemoveListener(OnClick);
        }


        public void Setup(PlayerController playerController, BallData data)
        {
            m_playerController = playerController;
            Data = data;

            m_iconImage.sprite = data.DisplayIcon;
        }


        public void ResetCooldown()
        {
            SetCooldown(Data.UseCooldown);
        }

        public void SetCooldown(int cooldown)
        {
            Cooldown = cooldown;
            m_skipCountdown = true;

            UpdateCooldownUI();
        }

        public void CountdownCooldown()
        {
            if(m_skipCountdown)
            {
                m_skipCountdown = false;
                return;
            }

            if (Cooldown <= 0)
                return;

            Cooldown--;
            UpdateCooldownUI();
        }

        private void UpdateCooldownUI()
        {
            m_onCooldownOverlay.SetActive(Cooldown > 0);
            m_cooldownText.gameObject.SetActive(Cooldown > 0);

            m_cooldownText.text = $"{Cooldown}";
        }


        public void SetSelected(bool state)
        {
            m_selectionFrame.SetActive(state);
        }


        private void OnClick()
        {
            m_playerController.OnBallSelectionItemClicked(this);
        }
    }
}
