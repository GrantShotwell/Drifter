using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {
    Button Start_Button, Level_Editor_Button, Settings_Button;
    
    void Start() {
        Button StartButton = Start_Button.GetComponent<Button>();
        Button EditorButton = Start_Button.GetComponent<Button>();
        Button SettingsButton = Start_Button.GetComponent<Button>();
        StartButton.onClick.AddListener(ClickedStart);
        EditorButton.onClick.AddListener(ClickedStart);
        SettingsButton.onClick.AddListener(ClickedStart);
    }

    void ClickedStart() {
        SceneManager.LoadScene("Test Level 1", LoadSceneMode.Single);
    }
    void ClickedEditor() {

    }
    void ClickedSettings() {

    }
}
