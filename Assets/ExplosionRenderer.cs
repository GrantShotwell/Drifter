using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRenderer : MonoBehaviour {
    new public GameObject light;
    Normal2D.NormalSource normalSource;
    float originalSize;
    public float speed = 2.0f;
    public bool editMaterialMode = false;
    new SpriteRenderer renderer;
    float start;

    private void Start() {
        start = Time.time;
        renderer = GetComponent<SpriteRenderer>();
        if(light != null) {
            normalSource = light.GetComponent<Normal2D.NormalSource>();
            originalSize = normalSource.radius;
        }
    }

    private void Update() {
        if(!editMaterialMode) {
            Material material = new Material(renderer.sharedMaterial);

            float progress = (Time.time - start) * speed;

            if(progress > 1) Destroy(gameObject);

            material.SetFloat("_Progress", progress);
            if(light != null) {
                Color lightColor = Color.Lerp(
                    material.GetColor("_ExpBright"),
                    material.GetColor("_ExpDark"),
                    progress
                );
                lightColor.a = 1 - progress;
                light.GetComponent<Light2D.LightSprite>().Color = lightColor;

                if(normalSource != null) normalSource.radius = originalSize / progress;
            }

            renderer.sharedMaterial = material;
        }
    }
}
