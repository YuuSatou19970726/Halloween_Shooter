using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    // Joystick rotation
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject parentCamera;

    private float rotHAmt;
    private float rotVAmt;
    public float rotationSpeed = 35f;

    // Enemy Move Speed
    private float speed = 1;
    public static float step;

    // Shooting
    public GameObject Crosshair;
    public Animator armsAnim;
    public AudioSource gunShotSound;

    RaycastHit hit;
    private float targetDistance = 200f;

    [SerializeField]
    public LayerMask targetExclude;

    WaitForSeconds zombieDeathTime = new WaitForSeconds(1.5f);
    WaitForSeconds batDeathTime = new WaitForSeconds(1f);
    WaitForSeconds ghostDeathTime = new WaitForSeconds(0.2f);

    // Particles
    [SerializeField]
    private GameObject zombieParticles, batParticles, ghostParticles;

    // Spawn Points
    public int zombiesOnScreen = 2;
    public int batsOnScreen = 2;
    public int ghostsOnScreen = 2;
    private GameObject lastDestroyedZombie, lastDestroyedBat, lastDestroyedGhost;
    [SerializeField]
    private Transform spawnPoint_1, spawnPoint_2, spawnPoint_3, spawnPoint_4;
    [SerializeField]
    private Transform playerTarget;

    private int spot = 1;
    private Transform nextAvailableSlot;
    WaitForSeconds spawnTime = new WaitForSeconds(1);

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        TimeCountDown();
    }

    // Update is called once per frame
    void Update()
    {
        JoystickRotate();
        EnemyMoveCalculation();
    }

    void JoystickRotate()
    {
        rotVAmt = joystick.Vertical;
        rotHAmt = joystick.Horizontal;

        parentCamera.transform.Rotate(0, rotHAmt * rotationSpeed * Time.deltaTime, 0);
        camera.transform.Rotate(-rotVAmt * rotationSpeed * Time.deltaTime, 0, 0);
    }

    void EnemyMoveCalculation()
    {
        step = speed * Time.deltaTime;
    }

    public void Shoot()
    {
        armsAnim.SetTrigger(AnimationTags.FIRE_TRIGGER);
        gunShotSound.Play();

        Ray ray = Camera.main.ScreenPointToRay(Crosshair.transform.position);
        if (Physics.Raycast(ray, out hit, targetDistance, targetExclude))
        {
            if (hit.transform.CompareTag(Tags.ZOMBIE_TAG))
            {
                hit.transform.gameObject.GetComponent<Animator>().SetTrigger(AnimationTags.DIE_TRIGGER);
                zombieParticles.transform.position = hit.transform.position;
                zombieParticles.SetActive(true);
                lastDestroyedZombie = hit.transform.gameObject;

                StartCoroutine(ZombieDeath());
            }

            if (hit.transform.CompareTag(Tags.BAT_TAG))
            {
                hit.transform.gameObject.GetComponent<Animator>().SetTrigger(AnimationTags.DIE_TRIGGER);
                batParticles.transform.position = hit.transform.position;
                batParticles.SetActive(true);
                lastDestroyedBat = hit.transform.gameObject;

                StartCoroutine(BatDeath());
            }

            if (hit.transform.CompareTag(Tags.GHOST_TAG))
            {
                ghostParticles.transform.position = hit.transform.position;
                ghostParticles.SetActive(true);
                lastDestroyedGhost = hit.transform.gameObject;

                StartCoroutine(GhostDeath());
            }
        }
    }

    IEnumerator ZombieDeath()
    {
        yield return zombieDeathTime;
        if (hit.transform.gameObject.activeInHierarchy)
            hit.transform.gameObject.SetActive(false);
        zombiesOnScreen--;
    }

    IEnumerator BatDeath()
    {
        yield return batDeathTime;
        if (hit.transform.gameObject.activeInHierarchy)
            hit.transform.gameObject.SetActive(false);
        batsOnScreen--;
    }

    IEnumerator GhostDeath()
    {
        yield return ghostDeathTime;
        if (hit.transform.gameObject.activeInHierarchy)
            hit.transform.gameObject.SetActive(false);
        ghostsOnScreen--;
    }

    public void Reload()
    {
        armsAnim.SetTrigger(AnimationTags.RELOAD_TRIGGER);
    }

    void TimeCountDown()
    {
        StartCoroutine(TimeDelay());
    }

    IEnumerator TimeDelay()
    {
        yield return spawnTime;
        TimeCountDown();
        CheckForEnemies();
    }

    void CheckForEnemies()
    {
        if (zombiesOnScreen < 2)
        {
            CheckSpots();
            if (lastDestroyedZombie != null)
            {
                lastDestroyedZombie.transform.position = nextAvailableSlot.position;
                if (spot < 4)
                    spot++;
                if (spot == 4)
                    spot = 1;
                lastDestroyedZombie.transform.LookAt(playerTarget);
                lastDestroyedZombie.SetActive(true);
                lastDestroyedZombie.GetComponent<EnemyMovement>().ZombieMoveTo();
                zombiesOnScreen++;
            }
        }

        if (batsOnScreen < 2)
        {
            CheckSpots();
            if (lastDestroyedBat != null)
            {
                lastDestroyedBat.transform.position = nextAvailableSlot.position;
                if (spot < 4)
                    spot++;
                if (spot == 4)
                    spot = 1;
                lastDestroyedBat.transform.LookAt(playerTarget);
                lastDestroyedBat.SetActive(true);
                batsOnScreen++;
            }
        }

        if (ghostsOnScreen < 2)
        {
            CheckSpots();
            if (lastDestroyedGhost != null)
            {
                lastDestroyedGhost.transform.position = nextAvailableSlot.position;
                if (spot < 4)
                    spot++;
                if (spot == 4)
                    spot = 1;
                lastDestroyedGhost.transform.LookAt(playerTarget);
                lastDestroyedGhost.SetActive(true);
                ghostsOnScreen++;
            }
        }
    }

    void CheckSpots()
    {
        switch (spot)
        {
            case 1:
                nextAvailableSlot = spawnPoint_1;
                break;
            case 2:
                nextAvailableSlot = spawnPoint_2;
                break;
            case 3:
                nextAvailableSlot = spawnPoint_3;
                break;
            case 4:
                nextAvailableSlot = spawnPoint_4;
                break;
        }
    }
}
