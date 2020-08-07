using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    [SerializeField]
    private GameObject SwitchingObject;

    public void ActiveSwitching()
    {
        SwitchingObject.SetActive(!SwitchingObject.activeSelf);
    }

    public void GamePause()
    {
        if (Time.timeScale > 0f) Time.timeScale = 0f;

        else Time.timeScale = 1f;

        Castle.Instnace.ShutDownSwitching();
    }

    public void Resume()
    {
        ActiveSwitching();

        GamePause();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
