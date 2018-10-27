using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEditor {
    GameObject tilemapParent;
    Tilemap tilemap;
    BoundsInt bounds;
    TileBase[] tileList;
    public TilemapEditor(GameObject tmp) {
        tilemapParent = tmp;
        tilemap = tilemapParent.GetComponent<Tilemap>();
        bounds = tilemap.cellBounds;
        tileList = tilemap.GetTilesBlock(bounds);
    }

    public void PlaceGameObject(string tileName, GameObject gObj) { PlaceGameObject(tileName, gObj, true);  }
    public void PlaceGameObject(string tileName, GameObject gObj, bool removeTile) { PlaceGameObject(tileName, gObj, removeTile, null); }
    public void PlaceGameObject(string tileName, GameObject gObj, bool removeTile, Transform parent) {
        for(int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++) {
            for(int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++) {
                Vector3Int localPlace = new Vector3Int(x, y, (int)tilemap.transform.position.z);
                if(tilemap.HasTile(localPlace)) {
                    if(tilemap.GetTile(localPlace).name == tileName) {
                        GameObject newObj = Object.Instantiate(gObj);
                        newObj.transform.position = tilemapParent.GetComponent<GridLayout>().CellToWorld(localPlace);
                        Vector3 gridSize = tilemapParent.GetComponent<Grid>().cellSize;
                        newObj.transform.position += gridSize / 2;
                        if(parent != null) newObj.transform.parent = parent;
                        if(removeTile) tilemap.SetTile(localPlace, null);
                    }
                }
            }
        }
    }

    public void Replace(string oldTileName, TileBase newTileObj) {
        for(int x = 0; x < bounds.size.x; x++) {
            for(int y = 0; y < bounds.size.y; y++) {
                TileBase tile = tileList[x + y * bounds.size.x];
                if(tile != null && tile.name.Equals(oldTileName)) {

                }
            }
        }
    }
    
    public void UpdateBounds() {
        bounds = tilemap.cellBounds;
        tileList = tilemap.GetTilesBlock(bounds);
    }
}

