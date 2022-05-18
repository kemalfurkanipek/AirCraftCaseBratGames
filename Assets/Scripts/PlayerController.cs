using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //These are Gameobjects in my project.
    public GameObject startButton,jy,speedBar,deathScene,playScene,succesfulScene;
   
    RaycastHit hit;
    
    public FixedJoystick joystick;

    //These are aircraft's values.
    private Rigidbody rb;
    public float forwardSpeed = 0;
    public float forwardSpeedMultiplier = 100;
    public float speedMultiplier = 1000;
    public float horizontalSpeed, verticalSpeed = 4;
    public float smoothness, rotationSmoothnees = 5;
    public float maxHoriRotation = 0.6f;
    public float maxVertiRotation = 0.06f;
    public float Hori,Verti;

    // These are Texts in my project.
    public Text textSpeed, textScore, textTime;

    // These are success and time values.
    private int score = 0;
    private int timeValue = 95;
    private int checkCounter = 11;
    
    // These are control values  in my project.
    private bool isGround,isDeath,timerStart = false;

    // These are Audios in my projects;
    public AudioSource checkAudio,checkAudio2;
    public AudioClip checkAudioClip, startEngineAudio,bangAudio,winAudio;

    // I created this for speedController.
    public Slider speedSlider;

    

    void Start()
    {
        
        rb= GetComponent<Rigidbody>();

        InvokeRepeating("timerController", 1, 1);  // I used InvokeRepeating  for Timer and SpeedReducer.
        InvokeRepeating("speedReducer", 1, 1);
       
        
    }

  
    void Update()
    {

        JoystickController();  // This is Horizotal and Vertical Controller.
        HandlePlaneRotation(); // This is aircraft's rotation controller.
        checkPointController(); // This is RayCast controller for checkPoint.
        comingGameOverScene();  // I am using this method for gameoverscene. 
        successfulScene(); // I am using this method for successfullscene. 


    }
    

    private void FixedUpdate()
    {
        HandlePlaneMove();
    }

    public void JoystickController()
    {
        if (Input.GetMouseButton(0) || Input.touches.Length != 0)
        {
            Hori = joystick.Horizontal;
            Verti = joystick.Vertical;
        }
        else
        {
            Hori = Input.GetAxisRaw("Horizontal");
            Verti = Input.GetAxisRaw("Vertical");
        }
    }

    private void HandlePlaneRotation()
    {
        float horizontalRotation = Hori * maxHoriRotation;
        float verticalRotation = -Verti * maxVertiRotation;
        

        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(-verticalRotation, Hori*0.1f, -horizontalRotation, transform.rotation.w), Time.deltaTime * rotationSmoothnees) ;
        
    }

    private void HandlePlaneMove()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardSpeed * forwardSpeedMultiplier * Time.deltaTime);

        float xVelocity = Hori * speedMultiplier * horizontalSpeed * Time.deltaTime;
        float yVelocity = -Verti * speedMultiplier * verticalSpeed * Time.deltaTime;


        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(xVelocity,yVelocity,rb.velocity.z), Time.deltaTime*smoothness);
    }

    public void speedControl(float speed)   // I am controlling speed in here.
    {
        speed = speedSlider.value;
        forwardSpeed=speed;
        int speedStr = ((int)forwardSpeed)*10;
        textSpeed.text="Speed: "+speedStr.ToString();
    }

    public void checkPointController()
    {
        if (Physics.Raycast(transform.position, new Vector3(0, 0, 1), out hit, 300.0f))
        {

            if (hit.collider.gameObject.tag == "checkP")
            {
                hit.collider.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 0, 31);




            }
            
        }

    }

    private void OnTriggerEnter(Collider other)   // I am controlling collid and score
    {
        if (other.gameObject.tag == "checkP")
        {
            Destroy(other.gameObject);
            checkAudio.PlayOneShot(checkAudioClip);
            score += 10;
            textScore.text="Score: "+score.ToString();
            checkCounter--;
        }
    }

    public void timerController()   // I am controlling time in here.
    {
        if (timerStart == true) {
            timeValue--;
            textTime.text = "Time: " + timeValue.ToString();
        }
        
    }

    public void engineStart()  // I am using this method to start engine.
    {
        checkAudio.PlayOneShot(startEngineAudio);
        checkAudio2.Play();
        Destroy(startButton);
        jy.SetActive(true);
        speedBar.SetActive(true);
        if (forwardSpeed < 10)
        {
            forwardSpeed+=5;
            
        }

        timerStart = true;
        
       
    }

    public void speedReducer()
    {
        if (isGround==true&&forwardSpeed>1) { forwardSpeed -= 5; }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "gorund")
        {
            isGround = true;
        }

        if(collision.gameObject.tag == "deathArea")
        {
            isDeath = true;
            checkAudio.PlayOneShot(bangAudio);
        }
    }

    public void comingGameOverScene()
    {
        if (isDeath == true || timeValue==0)
        {
            deathScene.SetActive(true);
            playScene.SetActive(false);
            checkAudio2.Stop();
            
        }


    }

    public void successfulScene()
    {
        if (checkCounter == -11)
        {

            if (forwardSpeed > 1)
            {
                forwardSpeed -= 5;
            }

            playScene.SetActive(false);
            succesfulScene.SetActive(true);
            checkAudio2.Stop();
            checkAudio.PlayOneShot(winAudio);
            timeValue += 100;

            
        }
    }

    public void restartButton()   // I am using this method for restart in other scene.
    {
        SceneManager.LoadScene(0);
        playScene.SetActive(true);
        deathScene.SetActive(false);
        
    }

    





}
