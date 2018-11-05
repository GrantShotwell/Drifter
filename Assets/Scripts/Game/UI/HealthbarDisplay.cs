using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarDisplay : MonoBehaviour {
    [Header("Game Objects")]
    
    [Tooltip("The healthbar to track.")]
    public Healthbar healthbar;
    
    [Tooltip("The prefab UI element to display and animate.")]
    public GameObject heartPrefab;

    [Tooltip("The UI Canvas.")]
    public Canvas canvas;

    [Header("Settings")]

    [Tooltip("Distance between Heart Prefabs.")]
    public int space;

    private int currentMaxValue;
    GameObject[] hearts;

    private void Start() {
        currentMaxValue = healthbar.max;
        SetupHealthImages(currentMaxValue);
    }

    private void Update() {
        if(currentMaxValue != healthbar.max) {
            currentMaxValue = healthbar.max;
            SetupHealthImages(currentMaxValue);
        }
        for(int j = healthbar.health; j < currentMaxValue; j++) {
            hearts[j].GetComponent<CustomAnimator>().wait = false;
        }
        for(int j = healthbar.health - 1; j >= 0; j--) {
            hearts[j].GetComponent<CustomAnimator>().wait = true;
            hearts[j].GetComponent<CustomAnimator>().current = 0;
        }
    }

    void SetupHealthImages(int amount) {
        hearts = new GameObject[amount];
        Vector3 offset = new Vector3(-(heartPrefab.GetComponent<RectTransform>().rect.width * canvas.scaleFactor * amount) / 2, 0, 0);
        for(int j = 0; j < hearts.Length; j++) {
            hearts[j] = Instantiate(heartPrefab, gameObject.transform);
            RectTransform rt = hearts[j].GetComponent<RectTransform>();
            hearts[j].transform.localPosition = new Vector3(j * ((rt.rect.width * canvas.scaleFactor) + space), 0, 0);
        }
        foreach(GameObject heart in hearts) heart.transform.localPosition += offset;
    }
}
