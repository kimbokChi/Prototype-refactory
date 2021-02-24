using UnityEngine;

public class SystemMessage : Singleton<SystemMessage>
{
    [SerializeField] private GameObject Background;
    [SerializeField] private GameObject MessageBox;
    [SerializeField] private TMPro.TextMeshProUGUI MessageText;

    public void ShowMessage(string message)
    {
        Time.timeScale = 0f;

        Background.SetActive(true);
        MessageBox.SetActive(true);

        MessageText.text = message;

        SoundManager.Instance.PlaySound(SoundName.ErrorWindow);
    }
    public void CloseMessage()
    {
        Time.timeScale = 1f;

        Background.SetActive(false);
        MessageBox.SetActive(false);
    }
}
