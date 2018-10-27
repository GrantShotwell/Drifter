using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Ch Class
//do I keep ch lowercase in here, or do I keep a consistant naming system? :/
[ExecuteInEditMode]
public class ChClass : MonoBehaviour {
    public GameObject head, arms, scrn, face;

    private void Start() {
        head = transform.Find("head").gameObject;
        scrn = head.transform.Find("scrn").gameObject;
        face = scrn.transform.Find("face").gameObject;
        arms = transform.Find("arms").gameObject;
    }

    public static implicit operator GameObject(ChClass ch) { return ch.gameObject; }
    public static explicit operator ChClass(GameObject ch) { return ch.GetComponent<ChClass>(); }
}
#endregion

[ExecuteInEditMode]
public class ChController_s1r0n0 : MonoBehaviour {
    ChClass chClass;
    GameObject head => chClass.head;
    GameObject arms => chClass.arms;
    GameObject scrn => chClass.scrn;
    GameObject face => chClass.face;

    private void Start() {
        if(GetComponent<ChClass>() == null) gameObject.AddComponent<ChClass>();
        chClass = GetComponent<ChClass>();
    }
}
