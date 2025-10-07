using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    [SerializeField] Transform handTransform;
    private Item heldItem;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Item GetHeldItem() { return heldItem; }

    public void HoldItem(Item i)
    {
        StopHoldingItem();
        heldItem = i;

        HeldItemChanged();
        handTransform.localScale = Vector3.zero;
        handTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutSine);
    }

    public void StopHoldingItem()
    {
        if (heldItem == null) return;

        heldItem = null;

        handTransform.localScale = Vector3.one;
        handTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InSine).OnComplete(HeldItemChanged);
    }

    private void HeldItemChanged()
    {
        if (handTransform.childCount > 0) Destroy(handTransform.GetChild(0).gameObject);

        if(UIManager.instance.CheckLevel() == 0) CrossHairScript.ShowCrosshair();


        if (heldItem != null)
        {
            ItemSO so = Inventory.GetSOForItem(heldItem);
            if (so == null || so.heldItemPrefab == null) return;

            GameObject go = Instantiate(so.heldItemPrefab, handTransform.position, handTransform.rotation, handTransform);

            if (so.tags.Contains(ItemTags.Shrimp))
            {
                go.GetComponent<InactiveShrimp>().Construct(((ShrimpItem)GetHeldItem()).shrimp);
            }
        }
    }

    public void UseHeldItem()
    {
        if (handTransform.childCount == 0 || heldItem == null) return;

        //Transform obj = handTransform.GetChild(0).transform;
        //if (obj == null) return;

        //obj.DOKill();
        //obj.position = Vector3.zero;
        //obj.DOShakePosition(0.2f, 0.01f);
    }
}
