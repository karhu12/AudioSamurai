using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ControlRebind : MonoBehaviour
{
    public InputActionReference actionReference;
    public int defaultBindingIndex;

    private InputAction inputAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Button button;
    private Text text;

    void Start()
    {
        inputAction = actionReference.action;
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }

        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }

        button.onClick.AddListener(delegate { RemapButtonClicked(name, defaultBindingIndex); });
        ResetButtonMappingTextValue();
        
    }

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }

    void RemapButtonClicked(string name, int bindingIndex = 0)
    {
        inputAction.Disable();
        button.enabled = false;
        text.text = "Press any key...";

        rebindingOperation?.Dispose();
        rebindingOperation = inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => ButtonRebindCompleted());
        rebindingOperation.Start();
    }

    void ResetButtonMappingTextValue()
    {
        text.text = InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath);
        button.gameObject.SetActive(true);
    }

    void ButtonRebindCompleted()
    {
        inputAction.Enable();
        rebindingOperation.Dispose();
        rebindingOperation = null;
        ResetButtonMappingTextValue();
        button.enabled = true;
    }


}
