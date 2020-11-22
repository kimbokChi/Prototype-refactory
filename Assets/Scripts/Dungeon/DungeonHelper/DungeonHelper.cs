using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonHelper : MonoBehaviour
{
    [SerializeField] private ChargeGauge Charge;

    [SerializeField] private GameObject HandOut;
    
    [SerializeField] private Sprite NullImage;
    
    private bool mHasPlayer;

    private Item mContainItem;

    private void Reset()
    {
        Charge = FindObjectOfType(typeof(ChargeGauge)) as ChargeGauge;
    }

    private void Awake()
    {
        mHasPlayer = false;

        Charge.DisChargeEvent += Interact;

        HandOut.transform.position = transform.position + Vector3.forward;
        

        if (HandOut.TryGetComponent(out SpriteRenderer render))
        {
            Debug.Log("렌더러가 있네요?");
            Item rand = null; // = ItemLibrary.Instance.GetRandomItem();
            if (!rand)
            {
                Debug.Log("아이템이 읎어요");
                render.sprite = NullImage;
            }
            else
            {
                Debug.Log("아이템이 있어요");
                render.sprite = mContainItem?.Sprite;
            }
        }
        else Debug.Log("렌더러가 읎어요");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) mHasPlayer = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) mHasPlayer = false;
    }

    private void Interact()
    {
        if (mHasPlayer)
        {
            HandOut.SetActive(true);
            HandOut.GetComponent<HandOut>().Throw();
            Inventory.Instance.AddItem(mContainItem);
        }
    }
}
