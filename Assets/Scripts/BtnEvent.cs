using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnEvent : MonoBehaviour
{
    public Button btn1; // First button reference
    public Button btn2; // Second button reference

    void Start()
    {
        // Assign listeners to button clicks
        btn1.onClick.AddListener(OnButton1Click);
        btn2.onClick.AddListener(OnButton2Click);
    }

    // Method to handle Button 1 click
    void OnButton1Click()
    {

        // Disable Button 1 and enable Button 2
        btn1.interactable = false;
        btn2.interactable = true;
    }

    // Method to handle Button 2 click
    void OnButton2Click()
    {
        // Enable Button 1 and disable Button 2
        btn1.interactable = true;
        btn2.interactable = false;
    }
}
