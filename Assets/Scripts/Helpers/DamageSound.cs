using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSound : MonoBehaviour
{
    [SerializeField]
    private AudioSource damageSound;
    [SerializeField]
    private AudioClip zombieBite, batBite, ghostBite;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.ZOMBIE_TAG))
        {
            damageSound.clip = zombieBite;
            damageSound.Play();
            MainController.lastDestroyedZombie = other.gameObject;
            MainController.zombiesOnScreen--;
            MainController.zombieAttack = true;
        }
        if (other.CompareTag(Tags.BAT_TAG))
        {
            damageSound.clip = batBite;
            damageSound.Play();
            MainController.lastDestroyedBat = other.gameObject;
            MainController.batsOnScreen--;
            MainController.batAttack = true;
        }
        if (other.CompareTag(Tags.GHOST_TAG))
        {
            damageSound.clip = ghostBite;
            damageSound.Play();
            MainController.lastDestroyedGhost = other.gameObject;
            MainController.ghostsOnScreen--;
            MainController.ghostAttack = true;
        }
        other.gameObject.SetActive(false);
    }
}
