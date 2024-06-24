using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognitionRange : MonoBehaviour
{
    List<Item> itemsInRange;

    private void Awake()
    {
        itemsInRange = new List<Item>();
    }

    private void Update()
    {
        if (itemsInRange.Count != 0 && Input.GetKeyDown(KeyCode.F))
        {
            Vector3 playerPos = Player.Instance.transform.position;
            Item nearestItem = itemsInRange[0];
            for (int i = 0; i < itemsInRange.Count; ++i)
            {
                if (Vector3.Distance(playerPos, nearestItem.transform.position) >
                    Vector3.Distance(playerPos, itemsInRange[i].transform.position))
                {
                    nearestItem = itemsInRange[i];
                }
            }
            nearestItem.PickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            itemsInRange.Add(item);
            Player.Instance.ShowInteractionKey();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            itemsInRange.Remove(item);
            if (itemsInRange.Count == 0)
            {
                Player.Instance.HideInteractionKey();
            }
        }
    }
}
