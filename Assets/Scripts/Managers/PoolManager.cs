using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance = null;
    public static PoolManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public GameObject ItemPrefab;
    public Transform ItemContainer;
    List<Vector3> ItemPositions;
    List<Item> Item_List;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //InitItems();
        
    }

    //void InitItems()
    //{
    //    ItemPositions = new List<Vector3>() { 
    //        new Vector3(-8, 1.5f, 0),
    //        new Vector3(-7, 1.5f, 0),
    //        new Vector3(-4, 1.5f, 0),
    //        new Vector3(-3, 1.5f, 0),
    //        new Vector3(0, 1.5f, 0),
    //        new Vector3(1, 1.5f, 0)
    //    };
    //    Item_List = new List<Item>();

    //    for (int i=0; i<ItemPositions.Count; ++i)
    //    {
    //        GameObject itemObject = Instantiate(ItemPrefab, ItemContainer);
    //        Item item = itemObject.GetComponent<Item>();
    //        EnumItem[] allItems = (EnumItem[])Enum.GetValues(typeof(EnumItem));
    //        int randomIndex = UnityEngine.Random.Range(1, allItems.Length); // Exclude None
    //        item.uuid = allItems[randomIndex];
    //        item.transform.position = ItemPositions[i];
    //        Item_List.Add(item);
    //    }
    //}
}
