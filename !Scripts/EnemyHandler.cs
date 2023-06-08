using Pathfinding;
using System;
using System.Collections;
using UnityEngine;

public enum EnemyType { Moving, Teleport, Ranged, Flying }
[DefaultExecutionOrder(0)]
public class EnemyHandler : MonoBehaviour
{


    //Referances
    public Rigidbody2D m_Rigidbody2D;
    public Animator m_Animator;
    private StateMachine _stateMachine;


    [Header("Common")]
    public EnemyType m_EnemyType = EnemyType.Moving;
    public Transform m_Player;
    public float MoveSpeed;
    public bool IsFacingRight = true;
    [HideInInspector] public Vector3 startPos;

    [Header("Flying")]
    [HideInInspector] public AIPath m_AiPath;

    [Header("Teleport")]
    public float TeleportCooldown;
    public float TeleportAnimationDelay;

    [Header("Attacking")]
    public int MaxAttack = 2; 
    public float AttackCooldown = 1;
    public float AttackRange;
    public float AttackDamage;
    [SerializeField] private float attackAnimDelay;

    public float CastDistance = 1f;
    public Vector2 CastDirection = Vector2.right;
    public float CastWidth = 1f;
    public float CastHeight = 1f;
    [SerializeField] private LayerMask attackLayerMask;

    [Header("Ranged Attack Settings")]
    public GameObject SrrowPrefab;
    public Transform ShootPoint;
    public float ShootForce;

    [Header("Patroling")]
    public Transform[] PatrolWaypoints;
    public float WaypointStoppingDistance = 1f;

    [Header("Chasing")]
    public float ChaseRange;
    //Wall Jump
    public LayerMask ObstacleLayers;
    public float JumpForce = 10f;
    public float RaycastDistance = 3f;
    public Transform FeetTransform;


    private void Awake()
    {
        if (m_Player == null) m_Player = GameObject.FindGameObjectWithTag("Player").transform; //Setting from inspector is better
        startPos = transform.position;


        _stateMachine = new StateMachine();

        //States
        var chaseState = new ChaseState(this);
        var patrolingState = new PatrolState(this);
        var attackState = new AttackState(this);
        var teleportState = new TeleportState(this);
        var idleState = new IdleState(this);



        switch (m_EnemyType)
        {
            case EnemyType.Moving:


                At(patrolingState, chaseState, () => DistanceToPlayer() <= ChaseRange);
                At(chaseState, patrolingState, () => DistanceToPlayer() > ChaseRange);
                At(chaseState, attackState, () => DistanceToPlayer() <= AttackRange);
                At(attackState, chaseState, () => DistanceToPlayer() > AttackRange);

                //Start State
                _stateMachine.SetState(patrolingState);

                break;
            case EnemyType.Teleport:
                At(idleState, teleportState, () => DistanceToPlayer() <= ChaseRange);
                At(teleportState, idleState, () => DistanceToPlayer() > ChaseRange);
                At(teleportState, attackState, () => DistanceToPlayer() <= AttackRange && !teleportState._startAction);
                At(attackState, teleportState, () => DistanceToPlayer() > AttackRange);
                //Start State
                _stateMachine.SetState(idleState);
                break;
            case EnemyType.Ranged:
                At(idleState, attackState, () => DistanceToPlayer() <= AttackRange);
                At(attackState, idleState, () => DistanceToPlayer() > AttackRange);

                //Start State
                _stateMachine.SetState(idleState);
                break;
            case EnemyType.Flying:
                At(patrolingState, chaseState, () => DistanceToPlayer() <= ChaseRange);
                At(chaseState, patrolingState, () => DistanceToPlayer() > ChaseRange);
                At(chaseState, attackState, () => DistanceToPlayer() <= AttackRange);
                At(attackState, chaseState, () => DistanceToPlayer() > AttackRange);

                //Start State
                _stateMachine.SetState(patrolingState);
                break;
            default:
                break;
        }
        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
    }
    private void Start()
    {
        for (int i = 0; i < PatrolWaypoints.Length; i++) 
            PatrolWaypoints[i].SetParent(null);


        if (m_EnemyType == EnemyType.Flying)
        {
            m_AiPath = GetComponent<AIPath>();
        }
    }

 
    private void Update()
    {
        _stateMachine.Tick();
    }
    private void FixedUpdate() => _stateMachine.FixedTick();

    public float DistanceToPlayer() => (transform.position - m_Player.position).magnitude;

    public void Flip()
    {

        IsFacingRight = !IsFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void CheckAttack(int attackCount)
    {
        StartCoroutine(AttackAction(attackCount));
    }
    IEnumerator AttackAction(int attackCount )
    {
        yield return new WaitForSecondsRealtime(attackAnimDelay);

        Vector2 castPosition = (Vector2)transform.position + (Vector2)transform.right * CastDistance;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(castPosition, new Vector2(CastWidth, CastHeight), 0f, attackLayerMask);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.transform.TryGetComponent<IDamagable>(out IDamagable damageable))
            {
                damageable.Damage(AttackDamage * attackCount);
                break;
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (m_EnemyType != EnemyType.Ranged)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ChaseRange);
        }


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRange);


        Gizmos.color = Color.red;
        Vector2 castPosition = (Vector2)transform.position + (Vector2)transform.right * CastDistance;
        Gizmos.DrawWireCube(castPosition, new Vector3(CastWidth, CastHeight, 0f));
    }
}
