using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private readonly EnemyHandler _enemyHandler;
    public IdleState(EnemyHandler enemyHandler)
    {
        _enemyHandler = enemyHandler;
    }
    public void FixedTick()
    {
    }

    public void OnEnter()
    {
        if (_enemyHandler.m_EnemyType == EnemyType.Teleport)
        {
            _enemyHandler.transform.position = _enemyHandler.startPos;
            Debug.Log("change pos STATE");
        }
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
