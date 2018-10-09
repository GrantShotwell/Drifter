using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileObjectTranslator : MonoBehaviour {
    [Header("Settings")]

    [Tooltip("When placing the objects, delete the original tiles they are placed on.")]
    public bool removeTiles = true;

    [Header("Key")]

    public string[] tileNames;

    public GameObject[] gameObjects;

    TilemapEditor editor;

    void Start () {
        editor = new TilemapEditor(gameObject);
        for(int j = 0; j < tileNames.Length; j++)
            editor.PlaceGameObject(tileNames[j], gameObjects[j], removeTiles, gameObject.transform);
    }
}
