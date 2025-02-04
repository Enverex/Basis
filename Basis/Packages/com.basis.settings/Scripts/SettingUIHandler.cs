using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SettingUIHandler<T> : MonoBehaviour
{
    public BasisISettingHandler<T> setting;
    public TMP_InputField inputField;
    public Dropdown dropdown;

    // For generic settings, this will update the value based on the input field or dropdown selection
    public void SetUp(SettingType type)
    {
        if (type == SettingType.InputField)
        {
            inputField.onValueChanged.AddListener(OnValueChangedFromInput);
            inputField.text = setting.GetCurrentValue().ToString();
        }
        else if (type == SettingType.Dropdown)
        {
            dropdown.onValueChanged.AddListener(OnValueChangedFromDropdown);
            dropdown.value = int.Parse(setting.GetCurrentValue().ToString());
        }
    }

    private void OnValueChangedFromInput(string value)
    {
        setting.SetValue((T)Convert.ChangeType(value, typeof(T)));
        setting.ApplySetting();
    }

    private void OnValueChangedFromDropdown(int value)
    {
        setting.SetValue((T)Convert.ChangeType(value, typeof(T)));
        setting.ApplySetting();
    }
    public enum SettingType
    {
        InputField,
        Dropdown
    }
}
