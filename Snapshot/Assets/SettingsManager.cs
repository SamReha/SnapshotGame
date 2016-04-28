using UnityEngine;
using System.Collections;

public class SettingsManager : MonoBehaviour {
    public GameObject settingsPanel;
    public GameObject pausePanel;
    public GameObject audioPanel;
    public GameObject controlPanel;

    void Awake() {
        settingsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlPanel.SetActive(false);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void enterSettings() {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);

    }

    public void exitSettings() {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void enterAudioSettings() {
        audioPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void exitAudioSettings() {
        audioPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void enterControlPanel() {
        controlPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void exitControlPanel() {
        controlPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}
