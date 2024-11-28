using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab; // The bullet prefab
    public Transform firePoint; // The point from which bullets are fired
    public float bulletSpeed = 20f; // Speed of the bullets
    public int ammoCount = 10; // Current ammo count
    public int maxAmmo = 10; // Maximum ammo capacity
    public float reloadTime = 2f; // Time to reload
    public ParticleSystem muzzleFlash; // Muzzle flash particle system
    public AudioClip shootSound; // Shooting sound
    public AudioClip reloadSound; // Reloading sound
    public AudioSource audioSource; // Audio source for playing sounds
    public Camera playerCamera; // Reference to the player's camera
    public float aimFOV = 40f; // Field of view when aiming
    private float defaultFOV; // Default field of view
    private bool isReloading = false; // Is the gun currently reloading?

    void Start()
    {
        // Save the default field of view
        if (playerCamera != null)
            defaultFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        // Check for aiming input (right mouse button)
        if (Input.GetButton("Fire2"))
        {
            Aim(true);
        }
        else
        {
            Aim(false);
        }

        // Check for shooting input (left mouse button) and if not reloading
        if (Input.GetButtonDown("Fire1") && ammoCount > 0 && !isReloading)
        {
            Shoot();
        }

        // Check for reload input (R key) and if not already reloading
        if (Input.GetKeyDown(KeyCode.R) && ammoCount < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Add velocity to the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;

        // Reduce the ammo count
        ammoCount--;

        // Play muzzle flash effect
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Play shooting sound
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        // Destroy the bullet after 2 seconds to save resources
        Destroy(bullet, 2f);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // Play reload sound
        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        // Wait for the reload time
        yield return new WaitForSeconds(reloadTime);

        // Replenish ammo
        ammoCount = maxAmmo;

        isReloading = false;
    }

    void Aim(bool isAiming)
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = isAiming ? aimFOV : defaultFOV;
        }
    }
}
