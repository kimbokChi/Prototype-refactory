using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAP : Singleton<IAP>
{
    //bool IAp = false;
    // Start is called before the first frame update


    public bool IAp
    {
        get;
        private set;
    }

   // 서버에서 불러온 bool 값을 AiP에 적용하여 인앱 구매 했는지 안했는지 구분을한다 
    public void AiP(bool Aip)
    {
        APP = Aip;
    }
    public bool APP
    {
        get;
        private set;
    }
    // IAp 가 인앱결제시 성공할때 true 로 바뀌면 서버에 저장을 한다 
    //인앱 결제시 저장을 하기 위해 저장 함수 호출

    public void Reward()
    {
        IAp = true;
        APP = true;
        BackEndServerManager.Instance.SendDataToServerSchema("Player");
        print("인앱성공");
    }

    // Update is called once per frame
    public void Faild()
    {
       // IAp = false;
        print("인앱실패");
    }
}
