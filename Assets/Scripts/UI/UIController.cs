using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DieterDerVermieter
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_comboText;
        [SerializeField] private TMP_Text m_scoreText;


        private void Update()
        {
            if (GameValues.Combo <= 0) m_comboText.text = "";
            else m_comboText.text = $"{GameValues.Combo}x";

            m_scoreText.text = GameValues.Score.ToString("#,##");
        }
    }
}
