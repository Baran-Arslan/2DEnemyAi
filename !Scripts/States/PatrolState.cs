using UnityEngine;

public class PatrolState : IState
{
    private readonly EnemyHandler _enemyHandler;

    private float sqrStoppingDistance;
    private int currentWaypointIndex = 0;
    private Transform target;


    public PatrolState(EnemyHandler enemyHandler)
    {
        _enemyHandler = enemyHandler;
        sqrStoppingDistance = _enemyHandler.WaypointStoppingDistance * _enemyHandler.WaypointStoppingDistance;

    }

    public void OnEnter()
    {
        if (_enemyHandler.m_EnemyType == EnemyType.Flying) 
        {
            _enemyHandler.m_AiPath.enabled = false;
            _enemyHandler.m_Rigidbody2D.isKinematic = false;
        }

        if (_enemyHandler.m_EnemyType == EnemyType.Flying)
            _enemyHandler.transform.position = _enemyHandler.PatrolWaypoints[0].position;

        _enemyHandler.m_Animator.SetBool("Running", true);
        target = _enemyHandler.PatrolWaypoints[currentWaypointIndex];

        Vector2 direction = target.position - _enemyHandler.transform.position;
        if (direction.x > 0 && !_enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
        else if (direction.x < 0 && _enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
    }

    public void OnExit()
    {
        _enemyHandler.m_Animator.SetBool("Running", false);
    }

    public void Tick()
    {
        if (_enemyHandler.m_EnemyType == EnemyType.Flying) return;
        _enemyHandler.m_Animator.SetBool("Jump", _enemyHandler.m_Rigidbody2D.velocity.y > 3f);
    }
    public void FixedTick()
    {
        MoveToTarget();
        if (_enemyHandler.m_EnemyType == EnemyType.Flying) return;
        HandleJumpingWall();
    }


    private void MoveToTarget()
    {
        Vector2 direction = target.position - _enemyHandler.transform.position;
        float sqrDistanceToWaypoint = direction.sqrMagnitude;

        if (sqrDistanceToWaypoint < sqrStoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % _enemyHandler.PatrolWaypoints.Length;
            target = _enemyHandler.PatrolWaypoints[currentWaypointIndex];
            direction = target.position - _enemyHandler.transform.position; // Update direction after changing target

            if (direction.x > 0 && !_enemyHandler.IsFacingRight)
            {
                _enemyHandler.Flip();
            }
            else if (direction.x < 0 && _enemyHandler.IsFacingRight)
            {
                _enemyHandler.Flip();
            }
        }

        _enemyHandler.m_Rigidbody2D.velocity = new Vector2(direction.normalized.x * _enemyHandler.MoveSpeed, _enemyHandler.m_Rigidbody2D.velocity.y);
    }
    private void HandleJumpingWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(_enemyHandler.FeetTransform.position, _enemyHandler.transform.right, _enemyHandler.RaycastDistance, _enemyHandler.ObstacleLayers);
        Debug.DrawRay(_enemyHandler.FeetTransform.position, _enemyHandler.transform.right * _enemyHandler.RaycastDistance, Color.red);

        if (hit.collider != null && _enemyHandler.DistanceToPlayer() > 4)
        {
            _enemyHandler.m_Rigidbody2D.AddForce(Vector2.up * _enemyHandler.JumpForce, ForceMode2D.Impulse);
        }
        else if (_enemyHandler.m_Rigidbody2D.velocity.y > 0)
        {
            _enemyHandler.m_Rigidbody2D.velocity = new Vector2(_enemyHandler.m_Rigidbody2D.velocity.x, 0);
        }
    }


}
