using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSelection : MonoBehaviour
{
    [TextArea(1, 1)]
    [SerializeField]
    private string DungeonName;

    public string Name => DungeonName;

    public void DungeonEnter()
    {
        Debug.Log("Enter!");
        MainCamera.Instance.Fade(1.75f, FadeType.In, () => 
        {
            SceneManager.LoadScene(DungeonName);
        });
    }
}