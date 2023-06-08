using UnityEngine;

public class ChaseState : IState
{
    private readonly EnemyHandler _enemyHandler;

    Vector2 direction;


    public ChaseState(EnemyHandler enemyHandler)
    {
        _enemyHandler = enemyHandler;
    }
        
    public void OnEnter()
    {
        if (_enemyHandler.m_EnemyType == EnemyType.Flying)
        {
            _enemyHandler.m_AiPath.enabled = true;
            _enemyHandler.m_Rigidbody2D.isKinematic = true;
        }

        _enemyHandler.m_Animator.SetBool("Running", true);
    }

    public void OnExit()
    {
        _enemyHandler.m_Animator.SetBool("Running", false);

        if (_enemyHandler.m_EnemyType == EnemyType.Flying)
        {
            _enemyHandler.m_AiPath.enabled = false;
            _enemyHandler.m_Rigidbody2D.isKinematic = false;
        }
    }

    public void Tick()
    {
        RotateToTarget();
        if (_enemyHandler.m_EnemyType == EnemyType.Flying) return;
        _enemyHandler.m_Animator.SetBool("Jump", _enemyHandler.m_Rigidbody2D.velocity.y > 3f);
    }
    public void FixedTick()
    {
        if (_enemyHandler.m_EnemyType == EnemyType.Flying) return;

        MoveToTarget();
        HandleJumpingWall();
    }

    private void RotateToTarget()
    {
        direction = _enemyHandler.m_Player.position - _enemyHandler.transform.position;

        if (direction.x > 0 && !_enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
        else if (direction.x < 0 && _enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
    }
    private void MoveToTarget()
    {
        _enemyHandler.m_Rigidbody2D.velocity = new Vector2(direction.normalized.x * _enemyHandler.MoveSpeed, _enemyHandler.m_Rigidbody2D.velocity.y);
    }
    private void HandleJumpingWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(_enemyHandler.FeetTransform.position, _enemyHandler.transform.right, _enemyHandler.RaycastDistance, _enemyHandler.ObstacleLayers);
        Debug.DrawRay(_enemyHandler.FeetTransform.position, _enemyHandler.transform.right * _enemyHandler.RaycastDistance, Color.magenta);

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
