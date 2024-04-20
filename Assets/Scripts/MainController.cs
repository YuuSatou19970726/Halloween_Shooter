using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static int zombiesOnScreen = 2;
    public static int batsOnScreen = 2;
    public static int ghostsOnScreen = 2;
    public static GameObject lastDestroyedZombie, lastDestroyedBat, lastDestroyedGhost;
    [SerializeField]
    private Transform spawnPoint_1, spawnPoint_2, spawnPoint_3, spawnPoint_4;
    [SerializeField]
    private Transform playerTarget;

    private int spot = 1;
    private Transform nextAvailableSlot;
    WaitForSeconds spawnTime = new WaitForSeconds(1);

    // Player Damage
    public static bool zombieAttack = false;
    public static bool batAttack = false;
    public static bool ghostAttack = false;

    [SerializeField]
    private GameObject zombieGraphic, batGraphic, ghostGraphic;

    WaitForSeconds graphicPause = new WaitForSeconds(0.3f);

    // Health Bar
    [SerializeField]
    private Image healthBar;
    public float damageAmt = 0.05f;

    // Reloading
    [SerializeField]
    private GameObject reloadButton;
    private int bulletCount = 8;
    [SerializeField]
    private GameObject bullet_1, bullet_2, bullet_3, bullet_4, bullet_5, bullet_6, bullet_7, bullet_8;
    WaitForSeconds reloadPause = new WaitForSeconds(3);
    [SerializeField]
    private AudioSource reloadSound;

    // Timer
    [SerializeField]
    private Text timerText;
    public int levelTimeLimit = 30;
    private int levelCurrentTime;

    // Switching on Enemies
    [SerializeField]
    private GameObject zombie_1, zombie_2, bat_1, bat_2, ghost_1, ghost_2;

    // Level Up
    public static int levelNumber = 1;
    [SerializeField]
    private GameObject levelUp;
    [SerializeField]
    private Text levelText;

    // GameOver
    [SerializeField]
    private GameObject gameOver;
    public static bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        // Application.targetFrameRate = 60;
        if (gameEnded)
        {
            levelNumber = 1;
            gameEnded = false;
        }
        levelText.text = "Level " + levelNumber;
        speed = speed * (levelNumber / 2);
        levelCurrentTime = levelTimeLimit;
        TimeCountDown();
        StartCoroutine(LoadEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        JoystickRotate();
        EnemyMoveCalculation();
        DamageVisuals();
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

    void DamageVisuals()
    {
        if (zombieAttack)
        {
            zombieAttack = false;
            healthBar.fillAmount -= damageAmt;
            zombieGraphic.SetActive(true);
            StartCoroutine(GraphicOff());
        }
        if (batAttack)
        {
            batAttack = false;
            healthBar.fillAmount -= damageAmt;
            batGraphic.SetActive(true);
            StartCoroutine(GraphicOff());
        }
        if (ghostAttack)
        {
            ghostAttack = false;
            healthBar.fillAmount -= damageAmt;
            ghostGraphic.SetActive(true);
            StartCoroutine(GraphicOff());
        }
    }

    IEnumerator GraphicOff()
    {
        yield return graphicPause;
        if (zombieGraphic.activeInHierarchy)
            zombieGraphic.SetActive(false);
        if (batGraphic.activeInHierarchy)
            batGraphic.SetActive(false);
        if (ghostGraphic.activeInHierarchy)
            ghostGraphic.SetActive(false);
    }


    public void Shoot()
    {
        if (bulletCount > 0)
        {
            armsAnim.SetTrigger(AnimationTags.FIRE_TRIGGER);
            gunShotSound.Play();

            Ray ray = Camera.main.ScreenPointToRay(Crosshair.transform.position);

            bulletCount--;
            DisplayBullets();

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

                if (hit.transform.CompareTag(Tags.PUMPKIN_TAG))
                {
                    healthBar.fillAmount = 1;
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
    }

    void DisplayBullets()
    {
        if (bulletCount == 8)
        {
            bullet_1.SetActive(true);
            bullet_2.SetActive(true);
            bullet_3.SetActive(true);
            bullet_4.SetActive(true);
            bullet_5.SetActive(true);
            bullet_6.SetActive(true);
            bullet_7.SetActive(true);
            bullet_8.SetActive(true);
        }
        else
        {
            switch (bulletCount)
            {
                case 0:
                    bullet_1.SetActive(false);
                    reloadButton.SetActive(true);
                    break;
                case 1:
                    bullet_2.SetActive(false);
                    break;
                case 2:
                    bullet_3.SetActive(false);
                    break;
                case 3:
                    bullet_4.SetActive(false);
                    break;
                case 4:
                    bullet_5.SetActive(false);
                    break;
                case 5:
                    bullet_6.SetActive(false);
                    break;
                case 6:
                    bullet_7.SetActive(false);
                    break;
                case 7:
                    bullet_8.SetActive(false);
                    break;
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
        StartCoroutine(WaitForReload());
        reloadButton.SetActive(false);
        reloadSound.Play();
    }

    IEnumerator WaitForReload()
    {
        yield return reloadPause;
        bulletCount = 8;
        DisplayBullets();
    }

    void TimeCountDown()
    {
        if (levelCurrentTime > 0)
        {
            levelCurrentTime -= 1;
            timerText.text = levelCurrentTime.ToString();
            StartCoroutine(TimeDelay());
        }

        if (levelCurrentTime < 2)
        {
            levelUp.SetActive(true);
        }

        if (levelCurrentTime <= 0)
        {
            levelNumber++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator TimeDelay()
    {
        yield return spawnTime;
        TimeCountDown();
        CheckForEnemies();
        CheckForGameOver();
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

    void CheckForGameOver()
    {
        if (healthBar.fillAmount <= 0 && levelCurrentTime > 2)
        {
            gameEnded = true;
            bulletCount = 0;
            DisplayBullets();
            gameOver.SetActive(true);
            Time.timeScale = 0;
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

    IEnumerator LoadEnemies()
    {
        yield return reloadPause;
        zombie_1.SetActive(true);
        bat_1.SetActive(true);

        yield return reloadPause;
        ghost_1.SetActive(true);
        zombie_2.SetActive(true);

        yield return reloadPause;
        bat_2.SetActive(true);
        ghost_2.SetActive(true);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
