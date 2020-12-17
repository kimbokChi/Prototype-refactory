using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonFaildUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI KillCount;
    [SerializeField] private TMPro.TextMeshProUGUI ClearTime;

    private void Awake()
    {
        int clearSec = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime % 60f);
        int clearMin = Mathf.FloorToInt(GameLoger.Instance.ElapsedTime / 60f);

        ClearTime.text = $"{clearMin:D2} : {clearSec:D2}";
        KillCount.text = $"{GameLoger.Instance.KillCount:D3} 마리";
    }

    public void ReTry()
    {
        MainCamera.Instance.Fade(2.25f, FadeType.In, () => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void BackToTown()
    {
        MainCamera.Instance.Fade(2.25f, FadeType.In, () => SceneManager.LoadScene(0));
    }
}
