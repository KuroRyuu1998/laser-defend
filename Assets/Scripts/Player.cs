using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.5f;
    [SerializeField]  int health = 200;

    [Header("Projectile")]
    [SerializeField] GameObject laserPerfab;
    [SerializeField] float projectileSpeed = 20f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [Header("Explosion")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float duration = 1f;
    [Header("Sound")]
    [SerializeField] AudioClip laserSound;
    [SerializeField] [Range(0, 1)] float laserSoundVolume = 0.3f;
    [SerializeField] AudioClip playerDeathSound;
    [SerializeField] [Range(0, 1)] float playerDeathSoundVolume = 0.6f;


    Coroutine firingCoroutine;
    float xMin, xMax, yMin, yMax;
    void Start()
    {
        SetUpMoveBoundaries();
    }

    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
    IEnumerator FireContinously()
    {
        while (true)
        {
            //This quaternion corresponds to "no rotation" - the object is perfectly aligned with the world or parent axes
            //vẫn không hiểu thì google Quaternion
            AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserSoundVolume);
            GameObject laser = Instantiate(laserPerfab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }


    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);

    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();

        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, duration);
        AudioSource.PlayClipAtPoint(playerDeathSound, Camera.main.transform.position, playerDeathSoundVolume);

    }

    public int GetHealth()
    {
        return health;
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCam = Camera.main;
        xMin = gameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
}

