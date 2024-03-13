using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    public float flySpeed, maxFlySpeed, jumpPower, crashSpeed, walkSpeed, maxWalkSpeed, rotateSpeed, fuelLevel, fuelMax;
    private bool isGrounded, longGrounded;
    private float horizontal;
    private float lastFrameVelocity;
    private bool isAlive = true;
    private bool isTrusting = false;
    [SerializeField] private ParticleSystem Trustparticle;
    [SerializeField] private ParticleSystem LeftTrustparticle;
    [SerializeField] private ParticleSystem RightTrustparticle;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject playerVisual;
    [SerializeField] private bool Android = true;


    [SerializeField] private ParticleSystem JumpParticle;
    [SerializeField] private ParticleSystem ExplosionParticle;

    [SerializeField] private Slider fuelSlider;
    [SerializeField] private GameObject speedMeterArrow;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        fuelSlider.maxValue = fuelMax;
        fuelSlider.value = fuelLevel;
    }

    void Update()
    {
        if (!Android)
        {
            GetMovementInput();
        }
        

        lastFrameVelocity = body.velocity.magnitude; //velocity van 1 frame geleden voor crash detectie

        if (!isGrounded) //speedmeter pijl draaien
        {
            speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, -180, body.velocity.magnitude / maxFlySpeed));
        }
        else
        {
            speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        if(isAlive)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //Jump als je op de grond bent
            {
                if (isGrounded)
                {
                    body.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                    JumpParticle.Play();
                    //shake camera with intensity 2 and time 0.5
                    CinemachineShake.Instance.ShakeCamera(5f, 1.0f);
                    isGrounded = false;
                }
            }

            if (longGrounded && fuelLevel < fuelMax) //bij tanken op de grond 2 fuel per seconde
            {
                fuelLevel += Time.deltaTime * 2;
                fuelSlider.value = fuelLevel;
            }
        }
    }

    public void GetMovementInput(int amount = 0) //input voor toetsenbord als je niet op telefoon zit
    {
        if(amount != 0)
        {
            horizontal = amount;
            return;
        }
        horizontal = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            if(longGrounded)
            {
                body.AddForce(-transform.up * 10); //blijven plakken op de planeet
            }

            if(isGrounded)
            {
                if (body.velocity.magnitude < maxWalkSpeed) //lopen als je op de grond bent
                {
                    body.AddForce(transform.right * horizontal * walkSpeed, ForceMode2D.Force);
                }
                StopTrustParticles();
                StopLeftTrustParticles();
                StopRightTrustParticles();
            }
            else //als je in de lucht bent
            { 
                FlyingFixedUpdate();
            }
        }
    }

    void FlyingFixedUpdate()
    {
        transform.Rotate(Vector3.forward * -horizontal * rotateSpeed * Time.deltaTime * 50); //draaien links rechts

        if (horizontal > 0) //start particles als je naar rechts draait
        {
            StartLeftTrustParticles();
        }
        else
        {
            StopLeftTrustParticles();
        }

        if (horizontal < 0) //start particles als je naar links draait
        {
            StartRightTrustParticles();
        }
        else
        {
            StopRightTrustParticles();
        }

        if (Input.GetKey(KeyCode.Space) && body.velocity.magnitude < maxFlySpeed && fuelLevel > 0 || Input.GetKey(KeyCode.W) && body.velocity.magnitude < maxFlySpeed && fuelLevel > 0 || isTrusting && body.velocity.magnitude < maxFlySpeed && fuelLevel > 0)
        {
            CinemachineShake.Instance.ShakeCamera(body.velocity.magnitude / 10, 0.1f);
            body.AddForce(transform.up * flySpeed * Time.deltaTime * 50); // main Truster
            fuelLevel -= Time.deltaTime;
            fuelSlider.value = fuelLevel;
            StartTrustParticles();
        }
        else
        {
            StopTrustParticles();
        }
    }
    public void JumpTrust()
    {
        if(isAlive)
        {
            if (isGrounded)
            {
                body.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                JumpParticle.Play();
                isGrounded = false;
            }
            isTrusting = true;
        }
    }

    public void StopTruster()
    {
        isTrusting = false;
    }


    void OnTriggerStay2D(Collider2D obj) //inside planet gravity
    {
        if (obj.CompareTag("Planet"))
        {
            body.drag = 0.2f;

            float distance = Mathf.Abs(obj.GetComponent<GravityPoint>().planetRadius - Vector2.Distance(transform.position, obj.transform.position));
            if (distance < 0.6f) //dicht bij planeet = IsGrounded
            {
                isGrounded = true;
                if (!longGrounded)
                {
                    StartCoroutine(LongGrounded());
                }

            }
            else
            {
                isGrounded = false;
                longGrounded = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D obj)//exit planet gravity
    {
        if (obj.CompareTag("Planet"))
        {
            body.drag = 0.02f;
        }
    }

    void OnCollisionEnter2D(Collision2D obj)//crash detection
    {
        if (obj.collider.CompareTag("Planet") && lastFrameVelocity > crashSpeed && !longGrounded)
        {
            Debug.Log("Crashed");
            ExplosionParticle.Play();
            StartCoroutine(Die());
        }
    }
    IEnumerator Die()
    {
        if (isAlive)
        {
            CinemachineShake.Instance.ShakeCamera(20f, 0.2f);
            isAlive = false;
            playerVisual.SetActive(false);
            body.velocity = Vector2.zero;

            yield return new WaitForSeconds(3);

            transform.position = respawnPoint.position;
            body.velocity = Vector2.zero;
            fuelLevel = fuelMax;
            fuelSlider.value = fuelLevel;
            playerVisual.SetActive(true);
            isAlive = true;
        }
    }
    
    public void goDie()
    {
        if(isAlive)
        {
            ExplosionParticle.Play();
            StartCoroutine(Die());
        }
    }

    void StopTrustParticles() // Stop loop and stop particles
    {
        Trustparticle.loop = false; 
        if (Trustparticle.isPlaying)
        {
            Trustparticle.Stop();
        }
    }

    void StartTrustParticles() // Start loop and start particles
    {
        if (!Trustparticle.isPlaying)
        {
            Trustparticle.Play();
        }
        Trustparticle.loop = true;
    }

    void StartLeftTrustParticles() //Left Start
    {
        if (!LeftTrustparticle.isPlaying)
        {
            LeftTrustparticle.Play();
        }
        LeftTrustparticle.loop = true;
    }

    void StopLeftTrustParticles() //Left Stop
    {
        LeftTrustparticle.loop = false; 
        if (LeftTrustparticle.isPlaying)
        {
            LeftTrustparticle.Stop();
        }
    }

    void StartRightTrustParticles() // Right Start
    {
        if (!RightTrustparticle.isPlaying)
        {
            RightTrustparticle.Play();
        }
        RightTrustparticle.loop = true;
    }

    void StopRightTrustParticles() // Right Stop
    {
        RightTrustparticle.loop = false; 
        if (RightTrustparticle.isPlaying)
        {
            RightTrustparticle.Stop();
        }
    }

    IEnumerator LongGrounded() //lang op de grond voor fuel tanken
    {
        yield return new WaitForSeconds(0.5f);
        longGrounded = true;
    }
}
