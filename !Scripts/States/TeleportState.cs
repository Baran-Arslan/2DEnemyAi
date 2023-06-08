using System.Collections;
using UnityEngine;


public class TeleportState : IState
{
    private readonly EnemyHandler _enemyHandler;

    private bool _canTeleport = true;
    private float _currentTeleportTime;

    public bool _startAction = false;
    private float _currentAnimationTime = 0;

    public TeleportState(EnemyHandler enemyHandler)
    {
        _enemyHandler = enemyHandler;
    }


    public void Tick()
    {
        _canTeleport = _currentTeleportTime >= _enemyHandler.TeleportCooldown;
        if (!_canTeleport) _currentTeleportTime += Time.deltaTime;
        else HandleTeleport();

        if (_startAction)
        {
            _currentAnimationTime += Time.deltaTime;
            if (_currentAnimationTime >= _enemyHandler.TeleportAnimationDelay) TeleportAction();
        }

        Vector2 direction = _enemyHandler.m_Player.position - _enemyHandler.transform.position;
        if (direction.x > 0 && !_enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
        else if (direction.x < 0 && _enemyHandler.IsFacingRight)
        {
            _enemyHandler.Flip();
        }
    }
    public void FixedTick()
    {
    }

    public void OnEnter()
    {
        _canTeleport = false;
        _startAction = false;
        _currentTeleportTime = 0;
        _currentAnimationTime = 0;
    }

    public void OnExit()
    {
    }

    private void HandleTeleport()
    {
        if (!PlayerController.instance.isGrounded) return;

        for (int i = 0; i < PlayerController.instance.Waypoints.Length; i++)
        {
            if (PlayerController.instance.Waypoints[i].IsAvaible)
            {

                _enemyHandler.m_Animator.SetTrigger("Teleport");
                _startAction = true;

                _currentTeleportTime = 0;
                _canTeleport = false;

                break;
            }
        }
    }
    private void TeleportAction()
    {

        for (int i = 0; i < PlayerController.instance.Waypoints.Length; i++)
        {
            if (PlayerController.instance.Waypoints[i].IsAvaible)
            {
                _enemyHandler.transform.position = PlayerController.instance.Waypoints[i].transform.position;
                break;
            }
        }
        _currentAnimationTime = 0;
        _startAction = false;

    }

}
