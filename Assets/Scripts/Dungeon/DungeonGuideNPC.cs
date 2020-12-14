using UnityEngine;

public class DungeonGuideNPC : MonoBehaviour
{
    [SerializeField] private bool IsWaitForEffectDisable;

    [SerializeField] private GameObject InteractionButton;
    [SerializeField] private GameObject DungeonSelectWindow;

    private bool mHasPlayer;

    private void Awake()
    {
        mHasPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) InteractionButton.SetActive(mHasPlayer = true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) InteractionButton.SetActive(mHasPlayer = false);
    }

    public void Interact()
    {
        if (IsWaitForEffectDisable)
        {
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position, () => 
            {
                DungeonSelectWindow.SetActive(!DungeonSelectWindow.activeSelf);
            });
        }
        else
        {
            EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position);

            DungeonSelectWindow.SetActive(!DungeonSelectWindow.activeSelf);
        }
    }
}
