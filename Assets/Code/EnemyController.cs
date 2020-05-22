using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
    public float Speed = 1;
    private GameObject m_Player;

    public List<Vector3> Path { get; set; } = new List<Vector3>();
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    public override void Tick()
    {
        if (CanSeePlayer())
            Path = new List<Vector3>{m_Player.transform.position};

        CheckArrivedToPoint();
    }

    private void CheckArrivedToPoint()
    {
        if (Path != null && Path.Count > 0)
        {
            var arrived = Vector3.Distance(transform.position, Path[0]) < 0.5;
            if (arrived)
                Path.RemoveAt(0);
        }
    }
    
    public override  void PhysicsTick()
    {
        if (Path != null && Path.Count > 0)
            MoveToPosition(Path[0]);
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
        
        if(Path == null || Path.Count == 0)
            return;
        
        Gizmos.DrawLine(transform.position, Path[0]);
        for (int i = 1; i < Path.Count; i++)
        {
            Gizmos.DrawLine(Path[i-1], Path[i]);
        }
    }
}
