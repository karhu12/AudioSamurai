// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""4542b133-3543-4937-b6cf-66819e839cc1"",
            ""actions"": [
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""27ba80e0-b0ae-45ef-aaa2-f50999674b4e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump Attack"",
                    ""type"": ""Button"",
                    ""id"": ""46abe219-8584-45b8-9a22-1d5e143b89a6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack Alt"",
                    ""type"": ""Button"",
                    ""id"": ""d7c62cf5-cf4a-44d8-b590-caac2c9b291e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump Attack Alt"",
                    ""type"": ""Button"",
                    ""id"": ""601caa64-0900-4318-97a1-021e1a3765a5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""867796f9-9da2-401a-abd8-2d9585d83b49"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4ef2c25-dac8-4a6d-b656-4847796e2780"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2135e84-6ac1-4061-95dc-1d126a151bdd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack Alt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee332d98-f239-4bae-9d00-3375adb8380c"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump Attack Alt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_JumpAttack = m_Player.FindAction("Jump Attack", throwIfNotFound: true);
        m_Player_AttackAlt = m_Player.FindAction("Attack Alt", throwIfNotFound: true);
        m_Player_JumpAttackAlt = m_Player.FindAction("Jump Attack Alt", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_JumpAttack;
    private readonly InputAction m_Player_AttackAlt;
    private readonly InputAction m_Player_JumpAttackAlt;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @JumpAttack => m_Wrapper.m_Player_JumpAttack;
        public InputAction @AttackAlt => m_Wrapper.m_Player_AttackAlt;
        public InputAction @JumpAttackAlt => m_Wrapper.m_Player_JumpAttackAlt;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @JumpAttack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttack;
                @JumpAttack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttack;
                @JumpAttack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttack;
                @AttackAlt.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackAlt;
                @AttackAlt.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackAlt;
                @AttackAlt.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttackAlt;
                @JumpAttackAlt.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttackAlt;
                @JumpAttackAlt.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttackAlt;
                @JumpAttackAlt.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJumpAttackAlt;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @JumpAttack.started += instance.OnJumpAttack;
                @JumpAttack.performed += instance.OnJumpAttack;
                @JumpAttack.canceled += instance.OnJumpAttack;
                @AttackAlt.started += instance.OnAttackAlt;
                @AttackAlt.performed += instance.OnAttackAlt;
                @AttackAlt.canceled += instance.OnAttackAlt;
                @JumpAttackAlt.started += instance.OnJumpAttackAlt;
                @JumpAttackAlt.performed += instance.OnJumpAttackAlt;
                @JumpAttackAlt.canceled += instance.OnJumpAttackAlt;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnAttack(InputAction.CallbackContext context);
        void OnJumpAttack(InputAction.CallbackContext context);
        void OnAttackAlt(InputAction.CallbackContext context);
        void OnJumpAttackAlt(InputAction.CallbackContext context);
    }
}
