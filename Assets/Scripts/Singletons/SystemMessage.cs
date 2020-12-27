using UnityEngine;

public class SystemMessage : Singleton<SystemMessage>
{
    [SerializeField] private GameObject MessageBox;
    [SerializeField] private TMPro.TextMeshProUGUI MessageText;

    public void ShowMessage(string message)
    {
        MessageBox.SetActive(true);

        MessageText.text = message;
    }
}
