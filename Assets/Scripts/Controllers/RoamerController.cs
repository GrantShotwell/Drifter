using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamerController : MonoBehaviour {
    #region Variables
    public GameObject corpse;
    new SpriteRenderer renderer;
    Color originalColor;

    #region Weapon
    [System.Serializable]
    public class roamerWeapon {
        public float speed;
        public float range;
    }
    public roamerWeapon weapon = new roamerWeapon();
    GameObject weaponObject;
    GameObject target;
    MeshRenderer mesh;
    #endregion
    #endregion

    #region Update
    private void Start() {
        renderer = GetComponent<SpriteRenderer>();
        originalColor = renderer.color;

        #region Weapon
        target = GameObject.Find("Player");
        weaponObject = transform.Find("Roamer Weapon").gameObject;
        mesh = weaponObject.transform.Find("Spotlight").GetComponent<MeshRenderer>();
        #endregion
    }

    private void Update() {
        #region Weapon
        if(target != null) {
            if(Vector2.Distance(target.transform.position, weaponObject.transform.position) > weapon.range)
                mesh.enabled = false;
            else if(!mesh.enabled) {
                var dir = target.transform.position - weaponObject.transform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                weaponObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                mesh.enabled = true;
            }
        }
        else mesh.enabled = false;
        #endregion
    }

    private void FixedUpdate() {
        #region Weapon
        if(target != null) {
            Vector3 vectorToTarget = target.transform.position - weaponObject.transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            weaponObject.transform.rotation = Quaternion.Slerp(weaponObject.transform.rotation, q, weapon.speed * Time.fixedDeltaTime);
        }
        #endregion
    }
    #endregion

    #region Functions
    public void OnDamage() {

    }

    public void OnDeath() {
        GameObject newCorpse = Instantiate(corpse);
        newCorpse.transform.position = transform.position;
        newCorpse.GetComponent<Rigidbody2D>().velocity = new Vector2(target.GetComponent<Rigidbody2D>().velocity.magnitude / 2, 0);
        newCorpse.GetComponent<Rigidbody2D>().velocity = newCorpse.GetComponent<Rigidbody2D>().velocity.Rotate(Vector2.SignedAngle(
            Vector2.right,
            target.GetComponent<Rigidbody2D>().velocity
        ));
        newCorpse.GetComponent<Rigidbody2D>().angularVelocity = 100;
        Destroy(gameObject);
    }
    #endregion
}
