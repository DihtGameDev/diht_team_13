using System;
using Code;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    
    public float MoveSpeed = 1;
    public float MaxSpeedRadius = 5;
    public float StopRadius = 0.1f;
    public GameObject Bullet;
    private BulletController m_BulletController;
    private Vector3 m_LookTarget;
    private int m_Collisions = 0;
    private Camera m_Camera;

    public bool UsePhysicsMovement
        => m_Collisions > 0;

    private Vector3 m_MoveVector = Vector3.zero;
    
    private Rigidbody m_Rigidbody;

    private LevelController m_LevelController;
    
    void Start()
    {
        m_BulletController = Bullet.GetComponent<BulletController>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Camera = Camera.main;
        m_LevelController = FindObjectOfType<LevelController>();
    }

    void Update()
    {
        LookAtMouse();

        if (Input.GetKey(KeyCode.Space))
        {
            m_MoveVector = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
        }
        else
        {
            Move();
        }
        
        if(Input.GetMouseButton(0))
            Fire();
        
        if(Input.GetKeyDown(KeyCode.R))
            OnReceiveDamage();
    }

    private void FixedUpdate()
    {
        MoveWithCollisions();
    }

    private void Fire()
    {
        if (m_BulletController.IsAvailable)
        {
            Bullet.transform.position = transform.position + transform.forward;
            Bullet.transform.rotation = transform.rotation;
            m_BulletController.Fire();
        }
    }

    private void LookAtMouse()
    {
        var cameraTransform = m_Camera.transform;
        var cos = Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(Vector3.down, cameraTransform.forward));
        
        var mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(cameraTransform.position,
            Vector3.ProjectOnPlane(cameraTransform.position, Vector3.up)) / cos;

        m_LookTarget = m_Camera.ScreenToWorldPoint(mousePosition);
        m_LookTarget.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(m_LookTarget - transform.position);
    }

    private void Move()
    {
        var mouseDistance = Vector3.Distance(transform.position, m_LookTarget);
        var currentSpeed = mouseDistance < StopRadius ? 0 :
            Math.Min(MaxSpeedRadius, (mouseDistance - StopRadius)) / MaxSpeedRadius * MoveSpeed;

        m_MoveVector = transform.forward * Time.fixedDeltaTime * currentSpeed;
        // Use physics movement only if we have collisions. physics movement is buggy.
        if(!UsePhysicsMovement)
            transform.position += m_MoveVector;
    }

    private void MoveWithCollisions()
    {
        if (UsePhysicsMovement)
            m_Rigidbody.MovePosition(transform.position + m_MoveVector);  
    }

    public void ResetState()
    {
        m_Collisions = 0;
    }

    public void OnReceiveDamage()
    {
        Debug.Log("Player Dead");
        ResetState();
        m_LevelController.ResetLevel();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, StopRadius);
        Gizmos.DrawWireSphere(transform.position, MaxSpeedRadius);
    }

    private void OnCollisionEnter(Collision other)
    {
        m_Collisions++;
    }

    private void OnCollisionExit(Collision other)
    {
        m_Collisions--;
    }
}
