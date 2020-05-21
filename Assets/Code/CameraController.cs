using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject m_Player;
    private Camera m_Camera;
    public float HorizontalBound;
    public float VerticalBound;
    public float CameraMoveSpeed = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        var screenPoint = m_Camera.WorldToScreenPoint(m_Player.transform.position);
        screenPoint.x = Math.Abs(screenPoint.x - Screen.width / 2);
        screenPoint.y = Math.Abs(screenPoint.y - Screen.height / 2);
        var horizontalSpeed = Math.Min(1, screenPoint.x / (Screen.width / 2 * VerticalBound));
        var verticalSpeed = Math.Min(1, screenPoint.y / (Screen.height / 2 * HorizontalBound));

        var moveVector = m_Player.transform.position - transform.position;
        moveVector.x *= horizontalSpeed * CameraMoveSpeed * Time.deltaTime;
        moveVector.y = 0;
        moveVector.z *= verticalSpeed * CameraMoveSpeed * Time.deltaTime;

        transform.position += moveVector;
    }
}
