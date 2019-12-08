using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
public class ControlRebind : MonoBehaviour
{
    public InputActionReference actionReference;
    public int defaultBindingIndex;

    private InputAction inputAction;
    private InputAction old;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Button button;
    private Text text;

    void Start()
    {

        inputAction = actionReference.action;

        string action = PlayerPrefs.GetString(inputAction.name, null);
        if (action != null)
        {
            inputAction.ApplyBindingOverride(action);
        }
        
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
        old = inputAction.Clone();
        FindObjectOfType<AudioManager>().Play("Click");
        inputAction.Disable();
        button.enabled = false;
        text.text = "Press any key...";
        foreach (var action in InputSystem.ListEnabledActions()) {
            
        }
        rebindingOperation?.Dispose();
        rebindingOperation = inputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => ButtonRebindCompleted())
            .Start();
    }

    void ResetButtonMappingTextValue()
    {
        text.text = InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath);

        var ac = inputAction.bindings.FirstOrDefault((item) => item.action == inputAction.name);
        string path = ac.effectivePath;

        PlayerPrefs.SetString(inputAction.name, path);
    }

    void ButtonRebindCompleted()
    {
        bool duplicate = false;
        foreach (var action in InputSystem.ListEnabledActions()) {
            if (action.bindings.First().effectivePath == inputAction.bindings.First().effectivePath) {
                duplicate = true;
                break;
            }
        }
        if (duplicate) {
            inputAction = old;
            FindObjectOfType<AudioManager>().Play("ClickDeny");
        }
        inputAction.Enable();
        rebindingOperation.Dispose();
        rebindingOperation = null;
        ResetButtonMappingTextValue();
        button.enabled = true;
    }


}
