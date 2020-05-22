using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : Controller
{
    public int MaxRicochetCount = 1;
    private int m_Ricochet;

    public float Speed = 1;
    public float LifeTime = 10;
    private float m_LifeTimeLeft = 10;

    private Rigidbody m_Rigidbody;

    public bool IsAvailable = true;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void Fire()
    {
        gameObject.SetActive(true);
        IsAvailable = false;
    }

    public void ResetState()
    {
        m_Ricochet = 0;
        IsAvailable = true;
        m_LifeTimeLeft = LifeTime;
        if (m_Rigidbody != null) 
            m_Rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void Tick()
    {
        transform.position += transform.forward * Time.fixedDeltaTime * Speed;

        m_LifeTimeLeft -= Time.deltaTime;
        if (m_LifeTimeLeft < 0)
            ResetState();
    }

    public Vector3 newForward;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        if (m_Ricochet >= MaxRicochetCount)
        {
            ResetState();
            return;
        }
        
        var normal = other.GetContact(0).normal;
        normal = Vector3.Project(-transform.forward, normal);
        newForward = (normal + normal + transform.forward).normalized;
        transform.forward = newForward;
        m_Ricochet++;

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("OnReceiveDamage");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*4);
        Gizmos.DrawLine(transform.position, transform.position + newForward*2);

    }
}
