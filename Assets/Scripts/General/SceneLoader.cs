using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] 
    private int LoadSceneIndex;

    public void SceneLoad()
    {
        SceneManager.LoadScene(LoadSceneIndex);
    }

    public void SceneLoad(int index)
    {
        SceneManager.LoadScene(index);
    }
}
