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

    WaitForSeconds zombieDeathTime = new WaitForSeconds(2f);
    WaitForSeconds batDeathTime = new WaitForSeconds(1f);
    WaitForSeconds ghostDeathTime = new WaitForSeconds(0.5f);

    //Particles
    [SerializeField]
    private GameObject zombieParticles, batParticles, ghostParticles;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
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

                StartCoroutine(ZombieDeath());
            }

            if (hit.transform.CompareTag(Tags.BAT_TAG))
            {
                hit.transform.gameObject.GetComponent<Animator>().SetTrigger(AnimationTags.DIE_TRIGGER);
                batParticles.transform.position = hit.transform.position;
                batParticles.SetActive(true);
                StartCoroutine(BatDeath());
            }

            if (hit.transform.CompareTag(Tags.GHOST_TAG))
            {
                ghostParticles.transform.position = hit.transform.position;
                ghostParticles.SetActive(true);
                StartCoroutine(GhostDeath());
            }
        }
    }

    IEnumerator ZombieDeath()
    {
        yield return zombieDeathTime;
        hit.transform.gameObject.SetActive(false);
    }

    IEnumerator BatDeath()
    {
        yield return batDeathTime;
        hit.transform.gameObject.SetActive(false);
    }

    IEnumerator GhostDeath()
    {
        yield return ghostDeathTime;
        hit.transform.gameObject.SetActive(false);
    }

    public void Reload()
    {
        armsAnim.SetTrigger(AnimationTags.RELOAD_TRIGGER);
    }


}
