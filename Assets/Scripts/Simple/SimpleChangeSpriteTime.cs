using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleChangeSpriteTime : MonoBehaviour
{
    public string changeSpriteName;
    private string originSpriteName;
    Coroutine ing;

    private void Start()
    {
        originSpriteName = this.GetComponent<UISprite>().spriteName;
    }
    public void Change()
    {
        if (ing != null)
            StopCoroutine(ing);
        ing = StartCoroutine(Change_C());
    }

    IEnumerator Change_C()
    {
        this.GetComponent<UISprite>().spriteName = changeSpriteName;
        yield return new WaitForSeconds(0.3f);
        this.GetComponent<UISprite>().spriteName = originSpriteName;
    }
}
