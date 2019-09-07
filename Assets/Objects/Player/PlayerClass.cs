using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Useful for getting the components of a player (done a lot) without having to do that whole Start() { GetComponent<...>(); } thing.
/// </summary>
public class PlayerClass : MonoBehaviour {
    public new Rigidbody2D rigidbody;
    public new BoxCollider2D collider;
    public new SpriteRenderer renderer;
    public LineRenderer line;
    public PlayerController controller;
    public Healthbar healthbar;
    public Rope rope;

    private void OnValidate() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        controller = GetComponent<PlayerController>();
        healthbar = GetComponent<Healthbar>();
        rope = GetComponentInChildren<Rope>();
    }

    /// <summary>Implicitly converts PlayerClass into GameObject.</summary>
    /// <param name="player">The PlayerClass.</param>
    public static implicit operator GameObject(PlayerClass player) { return player.gameObject; }

    /// <summary>Explicitly converts GameObject into PlayerClass.</summary>
    /// <param name="player">The GameObject.</param>
    public static explicit operator PlayerClass(GameObject player) { return player.GetComponent<PlayerClass>(); }
}
