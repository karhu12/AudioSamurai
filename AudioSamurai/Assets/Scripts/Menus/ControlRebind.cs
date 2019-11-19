using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
public class ControlRebind : MonoBehaviour
{
    public Button button;
    public Text text;
    public InputActionReference actionReference;
    public int defaultBindingIndex;
    public InputActionAsset inputActionAsset;

    private InputAction inputAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private bool isActive;

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
<<<<<<< HEAD

        var ac = inputAction.bindings.FirstOrDefault((item) => item.action == inputAction.name);
        string path = ac.effectivePath;

        PlayerPrefs.SetString(inputAction.name, path);
        button.gameObject.SetActive(true);
=======
        button.gameObject.SetActive(!isActive);
>>>>>>> 4ac341ea6fa3cf97f780f12abbc5864a7b6301ec
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
