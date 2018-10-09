using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    public float timeToExist;
    float startTime, finalTime;

    public bool fade = false;

    [System.Serializable]
    public class FadeProperties {
        [DefinedValues("Smooth", "Flash")]
        public string type = "Smooth";
        [ConditionalField("type", "Flash")] public int period;
        [ConditionalField("type", "Flash")] public int subPeriod;
        public Color fadeColor;
        public Color normalColor = Color.white;
        public float length;
    }
    [ConditionalField("fade")] public FadeProperties fadeProperties = new FadeProperties();
    bool smoothFade = false, flashFade = false;

	void Start () {
        switch(fadeProperties.type) {
            case "Smooth":
                smoothFade = true;
                break;
            case "Flash":
                flashFade = true;
                break;
        }
        fadeProperties.type = null;

        if(Time.inFixedTimeStep) startTime = Time.time;
        else startTime = Time.time;
        finalTime = startTime + timeToExist;
	}

	void Update () {
        if(Time.time > finalTime) Destroy(gameObject);
        else if(fade) {
            float timeLeft = finalTime - Time.time;
            if(timeLeft < fadeProperties.length) {
                if(flashFade) {
                    if(Time.frameCount % fadeProperties.period <= fadeProperties.subPeriod) {
                        GetComponent<SpriteRenderer>().color = fadeProperties.fadeColor;
                    }
                    else {
                        GetComponent<SpriteRenderer>().color = fadeProperties.normalColor;
                    }
                }
                if(smoothFade) {
                    GetComponent<SpriteRenderer>().color = Color.Lerp(
                        fadeProperties.normalColor,
                        fadeProperties.fadeColor,
                        1 - (timeLeft / fadeProperties.length)
                    );
                }
            }
        }
	}
}
