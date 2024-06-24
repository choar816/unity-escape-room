using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUse : UIDragDropItem
{
    // [SerializeField] GameObject itemDest;
    // [SerializeField] UISprite srcSprite;
    // UISprite destSprite;
    // Vector2 srcSpriteSize;
    // Vector3 srcPos;
    // Vector3 destPos;
    // float dist = 30;

    // protected override void Start()
    // {
    //     base.Start();
    //     srcSpriteSize = srcSprite.localSize;
    //     destSprite = itemDest.GetComponent<UISprite>();
    //     destPos = itemDest.transform.localPosition;
    // }
    // protected override void OnDragDropStart()
    // {
    //     base.OnDragDropStart();

    //     srcPos = transform.localPosition;
    //     srcSprite.width = destSprite.width;
    //     srcSprite.height = destSprite.height;
    // }

    // protected override void OnDrag(Vector2 delta)
    // {
    //     base.OnDrag(delta);

    //     if (Vector3.Distance(transform.localPosition, destPos) < dist)
    //     {
    //         destSprite.color = Color.blue;
    //     }
    //     else
    //     {
    //         destSprite.color = Color.black;
    //     }
    // }

    // protected override void OnDragDropEnd(GameObject surface)
    // {
    //     base.OnDragDropEnd(surface);

    //     // 드래그 성공
    //     if (Vector3.Distance(transform.localPosition, destPos) < dist)
    //     {
    //         transform.localPosition = destPos; // snap
    //         StartCoroutine(HideItemUseUiAfterDelay());
    //     }
    //     // 드래그 실패
    //     else
    //     {
    //         transform.localPosition = srcPos;
    //         srcSprite.width = (int)srcSpriteSize.x;
    //         srcSprite.height = (int)srcSpriteSize.y;
    //     }
    // }

    // IEnumerator HideItemUseUiAfterDelay()
    // {
    //     yield return new WaitForSeconds(1);
    //     UIManager.Instance.HideItemUseUI();
    // }

    // public void SetItemUseSprite(string spriteName)
    // {

    // }
}
