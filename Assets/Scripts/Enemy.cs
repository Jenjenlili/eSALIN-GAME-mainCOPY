using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int health;
    public int damage;
    public float range;
    public TextMeshProUGUI TagalogText;

    public EnemyTypes type;
    public LayerMask mcMask;
    public float atkCD;
    private bool canAtk = true;
    public Enemy targetEnemy;

    private void Start() {
        health = type.health;
        speed = type.speed;
        damage = type.damage;
        range = type.range;
        GetComponent<SpriteRenderer>().sprite = type.sprite;
    }
   
    private void Update() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, range, mcMask);

        if (hit.collider) {
            targetEnemy = hit.collider.GetComponent<Enemy>();
            Attack();
        }
        if (health == 1)
            GetComponent<SpriteRenderer>().sprite = type.deathsprite;
    }

    void Attack() {
        if (!canAtk || !targetEnemy)
            return;
        canAtk = false;
        Invoke("ResetAtkCooldown", atkCD);
    }

    private void FixedUpdate() {
        transform.position -= new Vector3(speed, 0, 0);
    }

    public void Hit(int damage) {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }

    // Method to set the Tagalog text on the enemy
    public void SetTagalogText(string tagalogWord) {
        if (TagalogText != null) {
            TagalogText.text = tagalogWord;
        } else {
            Debug.LogWarning("TagalogText TextMeshPro component not assigned.");
        }
    }

    private void ResetAtkCooldown() {
        canAtk = true;
    }
}