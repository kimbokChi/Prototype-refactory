using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionManager : Singleton<NPCInteractionManager>
{
    [System.Serializable]
    public struct InteractionUISet
    {
        public string Key;

        [Space()]
        public GameObject InteractionBtn;
        public GameObject InteractionObj;
    }

    [SerializeField] 
    private PlayerControllerSetting _PlayerController;

    [SerializeField]
    private InteractionUISet[] _InteractionUISets;
    
    private Dictionary<string, GameObject> _InteractionObjDic;

    private void Awake()
    {
        _InteractionObjDic = new Dictionary<string, GameObject>();

        for (int i = 0; i < _InteractionUISets.Length; i++)
        {
            var uiSet = _InteractionUISets[i];
            _InteractionObjDic.Add(uiSet.Key, uiSet.InteractionObj);
        }
        _PlayerController.ControllerSwapEvnt += con =>
        {
            bool enable = con == PlayerControllerSetting.Controller.Touch;

            for (int i = 0; i < _InteractionUISets.Length; i++)
            {
                _InteractionUISets[i].InteractionBtn.SetActive(enable);
            }
        };
    }
    public void SetActive(string key, bool active)
    {
        _InteractionObjDic[key]?.SetActive(active);
    }
}
