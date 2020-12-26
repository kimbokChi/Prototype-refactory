using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] 
    private int LoadSceneIndex;

    public void SceneLoad()
    {
        SceneManager.LoadScene(LoadSceneIndex);

        if (LoadSceneIndex == 1)
        {
            Inventory.Instance.Clear();
        }
    }

    public void SceneLoad(int index)
    {
        SceneManager.LoadScene(index);

        if (index == 1)
        {
            Inventory.Instance.Clear();
        }
    }
}
