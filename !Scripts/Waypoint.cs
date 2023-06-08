using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool IsAvaible;
    public bool isBlocked;
    public bool isGrounded;


    private void Update()
    {
        IsAvaible = !isBlocked && isGrounded;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) isGrounded = true;
        else isBlocked = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) isGrounded = false;
        else isBlocked = false;

    }
}