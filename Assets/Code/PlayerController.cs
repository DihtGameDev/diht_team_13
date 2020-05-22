using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 1;
    private Vector3 m_LookTarget;
    private Color[] m_Colors;

    void Start()
    {
    }

    void Update()
    {
        LookAtKeys();
        AlignedMove();

        //DrawTrace();
    }

    private void LookAtKeys()
    {
        var forward = 0;
        var right = 0;
        forward += Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        forward += Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        right += Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        right += Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;

        if (forward == 0 && right == 0)
            return;
        
        m_LookTarget = transform.position + Vector3.forward * forward + Vector3.right * right;
        transform.rotation = Quaternion.LookRotation(m_LookTarget - transform.position);
    }
    
    private void AlignedMove()
    {
        var forward = 0;
        var right = 0;
        forward += Input.GetKey(KeyCode.W) ? 1 : 0;
        forward += Input.GetKey(KeyCode.S) ? -1 : 0;
        right += Input.GetKey(KeyCode.D) ? 1 : 0;
        right += Input.GetKey(KeyCode.A) ? -1 : 0;

        transform.position += Vector3.forward * forward * Time.deltaTime * MoveSpeed;
        transform.position += Vector3.right * right * Time.deltaTime * MoveSpeed;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_LookTarget, 0.2f);
    }
}
