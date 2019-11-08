using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ControlRebind : MonoBehaviour
{

    public Button button;
    public Text text;
    public InputActionReference actionReference;
    public int defaultBindingIndex;
    public Button compositeButtons;
    public Text[] compositeTexts;

    private InputAction inputAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool isUsingComposite;

    InputAction action;
    public InputActionAsset inputActionAsset;


    void Start()
    {
        var inputActions = inputActionAsset.FindActionMap("Player");
        action = inputActions.FindAction("Attack");
        inputActionAsset.Enable();
        //inputAction = actionReference.action;
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }

        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }

        button.onClick.AddListener(delegate { RemapButtonClicked(name, defaultBindingIndex); });
        
    }

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }

    void RemapButtonClicked(string name, int bindingIndex = 0)
    {
        //InputAction actionToUse = inputAction.actionMap[0].actions[0];
        //inputAction.Disable();
        //actionToUse.Disable();
        /*button.enabled = false;
        text.text = "Press button";
        rebindingOperation?.Dispose();
        rebindingOperation = inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => ButtonRebindCompleted());
        rebindingOperation.Start();*/

        var inputActions = inputActionAsset.FindActionMap("Player");
        action = inputActions.FindAction("Attack");
        inputActionAsset.Disable();
        action.Disable();
        rebindingOperation = action.PerformInteractiveRebinding().WithRebindAddingNewBinding()
            .OnComplete(operation =>
            {
                action.Enable();
                inputActionAsset.Enable();
                ButtonRebindCompleted(GetComponent<Button>());
            })
            .Start();
    }

    void ResetButtonMappingTextValue()
    {
        text.text = InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath);
        button.gameObject.SetActive(!isUsingComposite);
    }

    void ButtonRebindCompleted(Button button)
    {
        rebindingOperation.Dispose();
        //rebindingOperation = null;
        ResetButtonMappingTextValue();
        button.enabled = true;
    }


}
