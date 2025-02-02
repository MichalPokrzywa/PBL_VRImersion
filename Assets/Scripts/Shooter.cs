using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePlace;
    public float force = 40f;
    bool isFiring = false;

    private void Update()
    {
        if (!isFiring)
        {
            StartCoroutine(FireRoutine());
        }
    }
    void Fire() {
        GameObject bullet = Instantiate(bulletPrefab, firePlace.position, firePlace.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePlace.right *force, ForceMode.Impulse);
        Destroy(bullet, 1.5f);
    }

    private IEnumerator FireRoutine()
    {
        isFiring = true;
        Fire();
        yield return new WaitForSeconds(1f); 
        isFiring = false;
    }
}
