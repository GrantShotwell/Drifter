using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileObjectTranslator : MonoBehaviour {
    TilemapEditor editor;
    public string[] tileNames;
    public GameObject[] gameObjects;

    void Start () {
        editor = new TilemapEditor(gameObject);
        for(int j = 0; j < tileNames.Length; j++)
            editor.PlaceGameObject(tileNames[j], gameObjects[j]);
    }
}
