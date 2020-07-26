// GENERATED AUTOMATICALLY FROM 'Assets/Input/UInputAction.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @UInputAction : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @UInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UInputAction"",
    ""maps"": [
        {
            ""name"": ""System"",
            ""id"": ""4ba557fb-409d-4bcf-bea7-3aaa9794cb96"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""41b02ee8-d5be-4725-937a-178fa85348bb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9d027136-986b-4447-ba49-c124c093fdf1"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": ""SlowTap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerControl"",
            ""id"": ""8edcfa3f-b4c1-4988-a949-fdad3e4d6089"",
            ""actions"": [
                {
                    ""name"": ""Input"",
                    ""type"": ""Button"",
                    ""id"": ""ae125578-6c1e-4ca3-8479-a823e1a0b834"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1ec5ec54-7d03-4f8b-987c-cad8c85f5405"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85f2eb59-aa82-4bd6-9ca7-2146b764c695"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae1fa77c-a1e2-4843-ade0-0f8730517832"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aecb95bc-cbea-4d5e-8833-3ed0d302c401"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerAction"",
            ""id"": ""23752274-4bf1-4a7c-b928-eccbc2719e61"",
            ""actions"": [
                {
                    ""name"": ""Item Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""0dec8439-f1cc-425d-89e6-37444982f487"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ea4b610b-0d49-437d-b3b9-98cac5817f48"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Input"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89cccd4f-a452-4fbe-92db-b5be942148b2"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // System
        m_System = asset.FindActionMap("System", throwIfNotFound: true);
        m_System_Newaction = m_System.FindAction("New action", throwIfNotFound: true);
        // PlayerControl
        m_PlayerControl = asset.FindActionMap("PlayerControl", throwIfNotFound: true);
        m_PlayerControl_Input = m_PlayerControl.FindAction("Input", throwIfNotFound: true);
        // PlayerAction
        m_PlayerAction = asset.FindActionMap("PlayerAction", throwIfNotFound: true);
        m_PlayerAction_ItemInteraction = m_PlayerAction.FindAction("Item Interaction", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // System
    private readonly InputActionMap m_System;
    private ISystemActions m_SystemActionsCallbackInterface;
    private readonly InputAction m_System_Newaction;
    public struct SystemActions
    {
        private @UInputAction m_Wrapper;
        public SystemActions(@UInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_System_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_System; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SystemActions set) { return set.Get(); }
        public void SetCallbacks(ISystemActions instance)
        {
            if (m_Wrapper.m_SystemActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_SystemActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_SystemActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_SystemActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_SystemActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public SystemActions @System => new SystemActions(this);

    // PlayerControl
    private readonly InputActionMap m_PlayerControl;
    private IPlayerControlActions m_PlayerControlActionsCallbackInterface;
    private readonly InputAction m_PlayerControl_Input;
    public struct PlayerControlActions
    {
        private @UInputAction m_Wrapper;
        public PlayerControlActions(@UInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Input => m_Wrapper.m_PlayerControl_Input;
        public InputActionMap Get() { return m_Wrapper.m_PlayerControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerControlActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerControlActions instance)
        {
            if (m_Wrapper.m_PlayerControlActionsCallbackInterface != null)
            {
                @Input.started -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnInput;
                @Input.performed -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnInput;
                @Input.canceled -= m_Wrapper.m_PlayerControlActionsCallbackInterface.OnInput;
            }
            m_Wrapper.m_PlayerControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Input.started += instance.OnInput;
                @Input.performed += instance.OnInput;
                @Input.canceled += instance.OnInput;
            }
        }
    }
    public PlayerControlActions @PlayerControl => new PlayerControlActions(this);

    // PlayerAction
    private readonly InputActionMap m_PlayerAction;
    private IPlayerActionActions m_PlayerActionActionsCallbackInterface;
    private readonly InputAction m_PlayerAction_ItemInteraction;
    public struct PlayerActionActions
    {
        private @UInputAction m_Wrapper;
        public PlayerActionActions(@UInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @ItemInteraction => m_Wrapper.m_PlayerAction_ItemInteraction;
        public InputActionMap Get() { return m_Wrapper.m_PlayerAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionActions instance)
        {
            if (m_Wrapper.m_PlayerActionActionsCallbackInterface != null)
            {
                @ItemInteraction.started -= m_Wrapper.m_PlayerActionActionsCallbackInterface.OnItemInteraction;
                @ItemInteraction.performed -= m_Wrapper.m_PlayerActionActionsCallbackInterface.OnItemInteraction;
                @ItemInteraction.canceled -= m_Wrapper.m_PlayerActionActionsCallbackInterface.OnItemInteraction;
            }
            m_Wrapper.m_PlayerActionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ItemInteraction.started += instance.OnItemInteraction;
                @ItemInteraction.performed += instance.OnItemInteraction;
                @ItemInteraction.canceled += instance.OnItemInteraction;
            }
        }
    }
    public PlayerActionActions @PlayerAction => new PlayerActionActions(this);
    public interface ISystemActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IPlayerControlActions
    {
        void OnInput(InputAction.CallbackContext context);
    }
    public interface IPlayerActionActions
    {
        void OnItemInteraction(InputAction.CallbackContext context);
    }
}
