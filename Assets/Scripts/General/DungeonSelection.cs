using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSelection : MonoBehaviour
{
    [TextArea(1, 1)]
    [SerializeField]
    private string DungeonName;

    public void DungeonEnter()
    {
        SceneManager.LoadScene(DungeonName);
    }
}