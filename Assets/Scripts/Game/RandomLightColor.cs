using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightColor : MonoBehaviour {
    public Light2D.LightSprite lightSprite;

    private void Update() {
        lightSprite.Color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            lightSprite.Color.a
        );
    }
}
