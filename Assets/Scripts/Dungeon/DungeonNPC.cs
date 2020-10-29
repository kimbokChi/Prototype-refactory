using UnityEngine;

public class DungeonNPC : MonoBehaviour
{
    [SerializeField]
    private ChargeGauge Charge;

    [SerializeField]
    private GameObject DungeonSelectWindow;

    private bool mHasPlayer;

    private void Reset()
    {
        Charge = FindObjectOfType(typeof(ChargeGauge)) as ChargeGauge;
    }

    private void Awake()
    {
        mHasPlayer = false;

        Charge.DisChargeEvent += Interact;
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
        if (mHasPlayer) {
            DungeonSelectWindow.SetActive(true);
        }
    }
}
