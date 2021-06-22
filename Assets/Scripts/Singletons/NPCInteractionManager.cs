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

    public NPC LastEnableNPC { get; private set; }

    [SerializeField, Header("Controller Property")] 
    private PlayerControllerSetting _PlayerController;
    [SerializeField]
    private VirtualJoystick _VirtualJoystick;

    [SerializeField, Space()]
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
    
    public void SetNowEnableNPC(NPC npc, bool enable)
    {
        if (enable)
        {
            LastEnableNPC = npc;
        }
        else if (LastEnableNPC.Equals(npc))
        {
            LastEnableNPC = null;
        }
    }
    
    public void SetActive(string key, bool active)
    {
        _InteractionObjDic[key]?.SetActive(active);
    }
    public void VJoystick_SetCoreBtnMode(CoreBtnMode btnMode)
    {
        _VirtualJoystick.SetCoreBtnMode(btnMode);
    }
}
