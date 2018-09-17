using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour {
    public int max = 3;
    private int _overflow = 0;
    private int _health = 3;
    
    public int health { get { return _health; } }
    public int overflow { get { return _overflow; } }
    
    public void Add(int amount) {
        if(amount > 0) Heal(amount);
        if(amount < 0) Damage(amount);
    }
    void Subtract(int amount) { Add(-amount); }

    public void Damage(int amount) {
        _health -= amount;
        if(health < 0) {
            _overflow += health;
            _health = 0;
        }
        else _overflow = 0;
    }
    public void Heal(int amount) {
        _health += amount;
        if(health > max) {
            _overflow += health - max;
            _health = max;
        }
        else _overflow = 0;
    }
}
