using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class HoverWizard : MonoBehaviour
{
    public float flightSpeed = 2f;
    public DetectionZone attackDetetionZone;
    public List<Transform> waypoints;
    public float waypointReachedDistance = 0.1f;
    public Collider2D colliderOnDeath;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWaypoint;
    int waypointNumber = 0;

    public bool _hasTarget;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }
    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNumber];
    }

    private void OnEnable()
    {
        // damageable.damageableDeath += OnDeath();
    }

    void Update()
    {
        HasTarget = attackDetetionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    private void Flight()
    {
        Vector3 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector3.Distance(nextWaypoint.position, transform.position);

        rb.linearVelocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if (distance <= waypointReachedDistance)
        {
            waypointNumber++;

            if (waypointNumber >= waypoints.Count)
            {
                waypointNumber = 0;
            }

            nextWaypoint = waypoints[waypointNumber];
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;

        if (locScale.x > 0)
        {
            if (rb.linearVelocity.x < 0)
            {
                locScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        // facing left
        else
        {
            if (rb.linearVelocity.x > 0)
            {
                locScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath()
    {
        rb.gravityScale = 2f;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        colliderOnDeath.enabled = true;
    }
}
