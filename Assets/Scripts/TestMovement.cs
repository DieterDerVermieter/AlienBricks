using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [SerializeField] float m_width = 5.0f;
    [SerializeField] float m_frequency = 1.0f;


    private void Update()
    {
        transform.position = Vector3.right * Mathf.Sin(Time.time * (Mathf.PI / m_frequency)) * m_width * 0.5f;
    }
}
