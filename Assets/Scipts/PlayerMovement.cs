using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    public float flySpeed, maxFlySpeed, jumpPower, crashSpeed, walkSpeed, maxWalkSpeed, rotateSpeed, fuelLevel, fuelMax;
    private bool isGrounded, longGrounded;
    private float horizontal;
    private float lastFrameVelocity;

    [SerializeField] private ParticleSystem Trustparticle;
    [SerializeField] private ParticleSystem LeftTrustparticle;
    [SerializeField] private ParticleSystem RightTrustparticle;


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
        horizontal = Input.GetAxis("Horizontal"); //get A D or arrow left right

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            body.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);// simple jump
            JumpParticle.Play();
            isGrounded = false;
        }

        lastFrameVelocity = body.velocity.magnitude;

        if (longGrounded && fuelLevel < fuelMax)
        {
            fuelLevel += Time.deltaTime * 2;
            fuelSlider.value = fuelLevel;
        }

        if (!isGrounded)
        {
        speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, -180, body.velocity.magnitude / maxFlySpeed));
        }
        else
        {
            speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    void FixedUpdate()
    {
        if(isGrounded)
        {
            if (body.velocity.magnitude < maxWalkSpeed)
            {
                body.AddForce(transform.right * horizontal * walkSpeed, ForceMode2D.Force);
            }

            body.AddForce(-transform.up * 10);
            StopTrustParticles();
            StopLeftTrustParticles();
            StopRightTrustParticles();
        }
        else
        { 

            if (horizontal > 0 && fuelLevel > 0)
            {
                //rotate right
                body.AddForce(transform.right * rotateSpeed * Time.deltaTime * 50);
                StartLeftTrustParticles();
            }
            else
            {
                StopLeftTrustParticles();
            }

            if (horizontal < 0 && fuelLevel > 0)
            {
                body.AddForce(-transform.right * rotateSpeed * Time.deltaTime * 50);
                StartRightTrustParticles();
            }
            else
            {
                StopRightTrustParticles();
            }

            if (Input.GetKey(KeyCode.Space) && body.velocity.magnitude < maxFlySpeed && fuelLevel > 0 || Input.GetKey(KeyCode.W) && body.velocity.magnitude < maxFlySpeed && fuelLevel > 0)
            {
                body.AddForce(transform.up * flySpeed * Time.deltaTime * 50);
                fuelLevel -= Time.deltaTime;
                fuelSlider.value = fuelLevel;
                StartTrustParticles();
            }
            else
            {
                StopTrustParticles();
            }
        }
    }

    void OnTriggerStay2D(Collider2D obj) //inside planet gravity
    {
        if (obj.CompareTag("Planet"))
        {
            body.drag = 0.4f;

            float distance = Mathf.Abs(obj.GetComponent<GravityPoint>().planetRadius - Vector2.Distance(transform.position, obj.transform.position));
            if (distance < 0.6f)
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
            body.drag = 0.1f;
        }
    }

    void OnCollisionEnter2D(Collision2D obj)//crash detection
    {
        if (obj.collider.CompareTag("Planet") && lastFrameVelocity > crashSpeed && !longGrounded)
        {
            Debug.Log("Crashed");
            ExplosionParticle.Play();
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

    IEnumerator LongGrounded()
    {
        yield return new WaitForSeconds(0.5f);
        longGrounded = true;
    }
}
