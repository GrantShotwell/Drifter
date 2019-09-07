using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;
using MyStuff.GeometryObjects;

public class AreaTest : MonoBehaviour
{
    public Area area;

    private void Start() {

    }

    private void OnDrawGizmosSelected() {
        foreach(Box box in area.boxes) {
            ExtendedDebug.DrawBox(box);
        }
    }

}
