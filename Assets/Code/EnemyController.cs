using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed = 1;
    public float FireCooldown;
    private float CooldownLeft = 0;
    public GameObject BulletPrefab;

    public float PathUpdateCooldown = 4f;
    private GameObject m_Player;

    private List<Vector3> m_Path = new List<Vector3>();
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Player = GameObject.FindWithTag("Player");
        m_Path.Add(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        CooldownLeft -= Time.deltaTime;

        if (CanSeePlayer())
        {
            Fire();
            if(m_Path == null || m_Path.Count == 0)
                m_Path = new List<Vector3>{m_Player.transform.position};
            else
                m_Path[0] = m_Player.transform.position;
        }

        MoveNextPoint();
    }

    private void FixedUpdate()
    {
        if (m_Path != null && m_Path.Count > 0)
            MoveToPosition(m_Path[0]);
    }

    private void MoveNextPoint()
    {
        if (m_Path != null && m_Path.Count > 0)
        {
            var arrived = Vector3.Distance(transform.position, m_Path[0]) < 0.5;
            if (arrived)
                m_Path.RemoveAt(0);
        }
    }

    private void MoveToPosition(Vector3 position)
    {
        var direction = (position - transform.position).normalized;
        // transform.position += direction * Time.fixedDeltaTime * Speed;
        m_Rigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * Speed);
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = m_Player.transform.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit))
        {
            if (hit.collider.gameObject == m_Player)
                return true;
        }

        return false;
    }

    private void Fire()
    {
        if (CooldownLeft < 0)
        {
            CooldownLeft = FireCooldown;
        }
    }

    public void OnReceiveDamage()
    {
        gameObject.SetActive(false);        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SendMessage("OnReceiveDamage");
        }
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        if(CanSeePlayer())
            Gizmos.DrawLine(transform.position, m_Player.transform.position);
        
        if(m_Path == null || m_Path.Count == 0)
            return;
        
        Gizmos.DrawLine(transform.position, m_Path[0]);
        for (int i = 1; i < m_Path.Count; i++)
        {
            Gizmos.DrawLine(m_Path[i-1], m_Path[i]);
        }
    }
}
