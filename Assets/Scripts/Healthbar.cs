using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour {
    public int max = 3;
    private static int _health = 3;
    private static int _overflow = 0;
    
    public static int health { get { return health; } }
    public static int overflow { get { return _overflow; } }
    
    void Damage(int amount) {
        _health -= amount;
        if(health < 0) {
            _overflow += health;
            _health = 0;
        }
        else _overflow = 0;
    }
    void Heal(int amount) {
        _health += amount;
        if(health > max) {
            _overflow += health - max;
            _health = max;
        }
        else _overflow = 0;
    }
}
