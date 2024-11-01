using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyType", menuName = "Enemies")]
public class EnemyTypes : ScriptableObject
{
    public int health;
    public float speed;

    public int damage;
    public float range = .5f;
    public float atkCD = 1f;
    public Sprite sprite;
    public Sprite deathsprite;
}
