using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    public float countdown = 3;
    public Text score;
    public GameObject player;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject restartTextObject;
    public bool timer = false;
    public Text timerText;
    public GameObject[] lives;
    public float speed;
    Animator anim;
    public int jump;
    public AudioClip musicClipOne;

    public AudioClip musicClipTwo;
    public AudioClip musicClipThree;
    public AudioSource musicSource;
    
    private int scoreValue = 0;
    private int life;
    private bool death;
    private string sceneName;
    private Rigidbody2D rd2d;
    private SpriteRenderer flipher;
    private BoxCollider2D boxCollider2d;
    private Rigidbody2D rigidbody2d;
    [SerializeField] private LayerMask platformslayerMask;
    // Start is called before the first frame update
    void Start()
    {
        flipher= GetComponent<SpriteRenderer>();
        rd2d = GetComponent<Rigidbody2D>();
        score.text = "Score: " + scoreValue.ToString();
        anim = GetComponent<Animator>();
        Scene currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        life = lives.Length;
        winTextObject.gameObject.SetActive(false);
        loseTextObject.gameObject.SetActive(false);
        restartTextObject.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (scoreValue >= 4)
        {
                if (Input.GetKey(KeyCode.Space))
                {
                    GameRestart();
                }
        }
        if (timer == true)
        {
            if (countdown > 0)
            {
                countdown = countdown - Time.deltaTime;
            }
            else if (countdown <= 0 && life <= 0)
            {
                countdown = 0;
                timer = false;
                GameRestart();
            }
            else
            {
                countdown = 0;
                timer = false;
                SceneChange();
            }
        }
        if (death == true)
        {
            LoseStage();
                if (Input.GetKey(KeyCode.Space))
                {
                    GameRestart();
                }
        }
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                anim.SetInteger("State", 1);
                speed = 5;
                if (Input.GetKey(KeyCode.A))
                    {
                        flipher.flipX = true;
                    }
                else  
                    {
                        flipher.flipX = false;
                    }
                }
            else 
                {
                anim.SetInteger("State", 0);
                speed = 0;
                }
        }
    if (Input.GetKey(KeyCode.W))
        {
          anim.SetInteger("State", 2);
          speed = 5;
        }
     if(Input.GetKeyDown("escape"))
        {
            Application.Quit();
        } 
    }
    void RestartTimer(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("Game Restart in {0:00}:{1:00}", minutes, seconds);
    }
    void NextLevel(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("Next Level in {0:00}:{1:00}", minutes, seconds);
    }

    private void WinStage ()
    {
                musicSource.Stop();
                musicSource.PlayOneShot(musicClipTwo);
                winTextObject.gameObject.SetActive(true);
                restartTextObject.gameObject.SetActive(true);
    }
    private void LoseStage()
    {
            timer = true;
            RestartTimer(countdown);
            loseTextObject.gameObject.SetActive(true);
            musicSource.clip = musicClipThree;
            musicSource.Play();
            player.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            score.text = scoreValue.ToString();
            Destroy(collision.collider.gameObject);
            if (scoreValue >= 4)
            {
                if (sceneName == "SampleScene")
                {
                    winTextObject.gameObject.SetActive(false);
                    timer = true;
                    NextLevel(countdown);
                }
                else
                {
                    WinStage();
                }
            }
        }
        if (collision.collider.tag == "Enemy")
        {
            Destroy(collision.collider.gameObject);
            TakeDamage(1);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            if (IsGrounded() && Input.GetKey(KeyCode.W))
            {
                float jumpVelocity = 6f;
                rigidbody2d.velocity = Vector2.up * jumpVelocity; 
            }
        }
    }
    void SceneChange()
    {
        SceneManager.LoadScene("Level 2");
    }
    void GameRestart()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void TakeDamage(int damage)
    {
        if (life >= 1)
        {
            life -= damage;
            Destroy(lives[life].gameObject);
            if (life < 1 && scoreValue >= 4)
            {
                return;
            }
            else if (life < 1)
            {
                death = true;
            }
        }
    }
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast (boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down * .1f, platformslayerMask);
        return raycastHit2D.collider != null;
    }
    }
