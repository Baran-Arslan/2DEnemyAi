using UnityEngine;

[DefaultExecutionOrder(1)]
public class AttackState : MonoBehaviour ,IState
{
    private readonly EnemyHandler _enemyHandler;

    private int _currentAttackCount = 1;
    private bool _canAttack = true;
    private float _currentAttackTime;

    public AttackState(EnemyHandler enemyHandler)
    {
        _enemyHandler = enemyHandler;
    }

    public void OnEnter()
    {
        if (_enemyHandler.m_Rigidbody2D != null && _enemyHandler.m_Rigidbody2D.velocity.y >= 0) 
            _enemyHandler.m_Rigidbody2D.velocity = Vector2.zero;

        _currentAttackCount = 1;
        _currentAttackTime = 0;
    }
    public void OnExit()
    {

    }
    public void Tick()
    {
        Vector2 direction = _enemyHandler.m_Player.position - _enemyHandler.transform.position;

        if (direction.x > 0 && !_enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
        else if (direction.x < 0 && _enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }

        _canAttack = _currentAttackTime >= _enemyHandler.AttackCooldown;
        if (!_canAttack) _currentAttackTime += Time.deltaTime;
        else Attack();
    }
    public void FixedTick() {}

    private void Attack()
    {
        _currentAttackTime = 0;
        _canAttack = false;

        //ADD SOUND (PLAY DIFFRENT SOUND DEPENDS ON THE ATTACK COUNT MAYBE)
        _enemyHandler.m_Animator.CrossFadeInFixedTime("Attack" + _currentAttackCount, 0.1f);

        if (_enemyHandler.m_EnemyType != EnemyType.Ranged)
            _enemyHandler.CheckAttack(_currentAttackCount);
        else
            RangedAttack();

        //Reset
        if (_currentAttackCount < _enemyHandler.MaxAttack) _currentAttackCount++;
        else _currentAttackCount = 1;
    }

    private void RangedAttack()
    {
        //Change with object pooling later
        Arrow arrow = Instantiate(_enemyHandler.SrrowPrefab, _enemyHandler.ShootPoint.position, _enemyHandler.ShootPoint.rotation).GetComponent<Arrow>();
        arrow.EnemyHandler = _enemyHandler;

    }
}
