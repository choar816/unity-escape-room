using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    SpriteRenderer sr;
    public int doorNumber;
    public Door destDoor;
    public int roomNumber;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.interactionKey.SetActive(true);
            Player.Instance.m_Stat.canEnterDoor = true;
            Player.Instance.m_Stat.nearestDoorNumber = doorNumber;
            sr.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.interactionKey.SetActive(false);
            Player.Instance.m_Stat.canEnterDoor = false;
            sr.color = new Color(1, 1, 1, 1);
        }
    }
}
