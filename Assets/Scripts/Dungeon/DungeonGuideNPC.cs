using UnityEngine;

public class DungeonGuideNPC : MonoBehaviour
{
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
        DungeonSelectWindow.SetActive(!DungeonSelectWindow.activeSelf);

        EffectLibrary.Instance.UsingEffect(EffectKind.Twinkle, transform.position);
    }
}
