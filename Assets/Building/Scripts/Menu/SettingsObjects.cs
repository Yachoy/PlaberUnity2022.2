using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsObjects : MonoBehaviour
{
    public Text textChoise;
    public Text textWarning;
    public InputField xInput;
    public InputField yInput;


    public void setTX(string value) => xInput.text = value;
    public void setTY(string value) => yInput.text = value;

    public void setTSelect(string name) => textChoise.text = "Select: "+name;

    public void setTWarning(string name) => textWarning.text = name;


}
