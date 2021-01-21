using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.Dispatcher;
using UnityEngine.SceneManagement;
public class Title : MonoBehaviour
{
    private static Title instance;
    public GameObject touchStart;
    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    public void TouchStart()
    {

        BackEndServerManager.GetInstance().BackendTokenLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {

                SceneManager.LoadScene(2);
                Debug.Log("통과");
                if (result)
                {
                    
                    return;
                }
                else
                if (!error.Equals(string.Empty))
                {
                    Debug.Log("유저 정보 불러오기 실패");
                    
                    return;
                }

               
            });
        });

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
