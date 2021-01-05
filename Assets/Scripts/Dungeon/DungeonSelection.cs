using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSelection : MonoBehaviour
{
    [SerializeField] private DungeonSelectionInfo _SelectionInfo;

    public string Name => _SelectionInfo.AttachedSceneName;

    public void DungeonEnter()
    {
        MainCamera.Instance.Fade(1.75f, FadeType.In, () => 
        {
            SceneManager.LoadScene(_SelectionInfo.AttachedSceneName);
        });
    }
}