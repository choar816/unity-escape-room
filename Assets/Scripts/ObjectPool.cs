using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject PooledObject;
    public int PooledAmount = 100;
    public bool WillGrow = false;

    private List<GameObject> pooledObjects;
    private Transform uidrawingTransform;
    private Sprite[] drawBoardSprites;

    private void Awake()
    {
        pooledObjects = new List<GameObject>();
        uidrawingTransform = transform.parent;

        // Resources 폴더에서 스프라이트 배열을 로드
        drawBoardSprites = new Sprite[PooledAmount];
        for (int i = 0; i < PooledAmount; i++)
        {
            drawBoardSprites[i] = Resources.Load<Sprite>($"Sprites/DrawBoards/DrawBoard ({i + 1})");
        }

        for (int i = 0; i < PooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(PooledObject, uidrawingTransform);
            obj.SetActive(false);

            // 각 오브젝트의 SpriteRenderer에 스프라이트를 설정
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr && i < drawBoardSprites.Length)
            {
                sr.sprite = drawBoardSprites[i];
            }

            pooledObjects.Add(obj);
        }
    }

    void Start()
    {
        return;

        //pooledObjects = new List<GameObject>();
        //uidrawingTransform = transform.parent;

        //// Resources 폴더에서 스프라이트 배열을 로드
        //drawBoardSprites = new Sprite[PooledAmount];
        //for (int i = 0; i < PooledAmount; i++)
        //{
        //    drawBoardSprites[i] = Resources.Load<Sprite>($"Sprites/DrawBoards/DrawBoard ({i + 1})");
        //}

        //for (int i = 0; i < PooledAmount; i++)
        //{
        //    GameObject obj = (GameObject)Instantiate(PooledObject, uidrawingTransform);
        //    obj.SetActive(false);

        //    // 각 오브젝트의 SpriteRenderer에 스프라이트를 설정
        //    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        //    if (sr && i < drawBoardSprites.Length)
        //    {
        //        sr.sprite = drawBoardSprites[i];
        //    }

        //    pooledObjects.Add(obj);
        //}
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (WillGrow)
        {
            GameObject obj = (GameObject)Instantiate(PooledObject, uidrawingTransform);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    public GameObject GetObjectAtIndex(int index)
    {
        if (index >= 0 && index < pooledObjects.Count)
        {
            return pooledObjects[index];
        }
        return null;
    }

}
