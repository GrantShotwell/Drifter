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

    public void PlaceGameObject(string tileName, GameObject gObj) { PlaceGameObject(tileName, gObj, true); }
    public void PlaceGameObject(string tileName, GameObject gObj, bool removeTile) {
        for(int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++) {
            for(int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++) {
                Vector3Int localPlace = new Vector3Int(n, p, (int)tilemap.transform.position.y);
                if(tilemap.HasTile(localPlace)) {
                    if(tilemap.GetTile(localPlace)) {
                        GameObject newObj = Object.Instantiate(gObj);
                        newObj.transform.position = tilemapParent.GetComponent<GridLayout>().CellToWorld(localPlace);
                        Vector3 gridSize = tilemapParent.GetComponent<Grid>().cellSize;
                        newObj.transform.position += gridSize / 2;
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

