using System;
using System.Collections.Generic;
using System.Linq;
using Code;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    
    public float MoveSpeed = 1;
    public float MaxSpeedRadius = 5;
    public float StopRadius = 0.1f;
    public GameObject Bullet;
    private BulletController m_Bullet;
    private Vector3 m_LookTarget;
    private List<Collision> m_Collisions = new List<Collision>();
    private Camera m_Camera;

    public bool UsePhysicsMovement
        => m_Collisions.Any();

    private Vector3 m_MoveVector = Vector3.zero;
    
    private Rigidbody m_Rigidbody;

    private LevelController m_LevelController;
    
    void Start()
    {
        // Application.targetFrameRate = 90;
        m_Bullet = Bullet.GetComponent<BulletController>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Camera = Camera.main;
        m_LevelController = FindObjectOfType<LevelController>();
    }

    void Update()
    {
        LookAtMouse();
        
        if (!Input.GetKey(KeyCode.Space))
            Move();
        
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
        if (m_Bullet.IsAvailable)
        {
            Bullet.transform.position = transform.position + transform.forward;
            Bullet.transform.rotation = transform.rotation;
            m_Bullet.Fire();
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

    public void OnReceiveDamage()
    {
        Debug.Log("Player Dead");
        m_LevelController.ResetLevel();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, StopRadius);
        Gizmos.DrawWireSphere(transform.position, MaxSpeedRadius);
    }

    private void OnCollisionEnter(Collision other)
    {
        m_Collisions.Add(other);
    }

    private void OnCollisionExit(Collision other)
    {
        m_Collisions.Remove(other);
    }
}
