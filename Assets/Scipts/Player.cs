using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    public float flySpeed, maxFlySpeed, jumpPower, crashSpeed, walkSpeed, maxWalkSpeed, rotateSpeed, fuelLevel, fuelMax, oxygenLevel, oxygenMax;
    private bool isGrounded, longGrounded, outsideGravity, isInvisible;
    [SerializeField] private TextMeshProUGUI rubyCounterText;
    private float horizontal;
    private float lastFrameVelocity;
    public bool isAlive = true, canMove = false;
    private bool isTrusting = false;
    
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private ParticleSystem Trustparticle;
    [SerializeField] private ParticleSystem LeftTrustparticle;
    [SerializeField] private ParticleSystem RightTrustparticle;

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject playerVisual;
    private bool Android = false;


    [SerializeField] private ParticleSystem JumpParticle;
    [SerializeField] private ParticleSystem ExplosionParticle;
    [SerializeField] private ParticleSystem PoofParticle;


    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Slider oxygenSlider;


    [SerializeField] private GameObject speedMeterArrow;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        fuelSlider.maxValue = fuelMax;
        fuelSlider.value = fuelLevel;
        oxygenSlider.maxValue = oxygenMax;
        oxygenSlider.value = oxygenLevel;
    }

    public void enableMovement()
    {
        canMove = true;
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
            speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 180, body.velocity.magnitude / maxFlySpeed));
        }
        else
        {
            //lerp vanaf de current rotation naar 0    Pijl langzaam terugdraaien
            speedMeterArrow.transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(speedMeterArrow.transform.localEulerAngles.z, 0, Time.deltaTime * 5));
        }

        if(isAlive && canMove)
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
            //---------------------------------Fuel---------------------------------bij tanken op de grond oxygen en fuel
            if (longGrounded && fuelLevel < fuelMax)
            {
                fuelLevel += Time.deltaTime * fuelMax / 2;
                fuelSlider.value = fuelLevel;
            }
            if (fuelLevel > fuelMax) //max fuel
            {
                fuelLevel = fuelMax;
                fuelSlider.value = fuelLevel;
            }
            //---------------------------------Oxygen---------------------------------
            if (!outsideGravity && oxygenLevel < oxygenMax)
            {
                oxygenLevel += Time.deltaTime * oxygenMax / 2;
                oxygenSlider.value = oxygenLevel;
            }
            if (oxygenLevel > oxygenMax) //max oxygen
            {
                oxygenLevel = oxygenMax;
                oxygenSlider.value = oxygenLevel;
            }
            //--------------------------------------------------------------------------
            if (outsideGravity)
            {
                oxygenLevel -= Time.deltaTime;
                oxygenSlider.value = oxygenLevel;
            }
            if (oxygenLevel <= 0)
            {
                explode();
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

            if(isGrounded && canMove)
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
    public void JumpTrust() // Jump for mobile
    {
        if(isAlive && canMove)
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
            outsideGravity = false;
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
            outsideGravity = true;
        }
    }

    void OnCollisionEnter2D(Collision2D obj)//crash detection
    {
        if (obj.collider.CompareTag("Planet") && lastFrameVelocity > crashSpeed && !longGrounded)
        {
            explode();
        }
        if (obj.collider.CompareTag("Alien"))
        { 
            if (isAlive && !isInvisible)
            {
                PoofParticle.Play();
                StartCoroutine(Die());
            }
        }

        if (obj.collider.CompareTag("Ruby"))
        {
            GameManager.Instance.CollectedDiamonds++;
            rubyCounterText.text = GameManager.Instance.CollectedDiamonds.ToString();
            Destroy(obj.gameObject);
        }
        if (obj.collider.CompareTag("Collectable"))
        {
            obj.gameObject.SetActive(false);
        }
    }

    IEnumerator Die()
    {
        if (!isInvisible && isAlive)
        {
            StopLeftTrustParticles();
            StopRightTrustParticles();
            StopTrustParticles();
            isAlive = false;
            playerVisual.SetActive(false);
            body.velocity = Vector2.zero;

            yield return new WaitForSeconds(3);

            transform.position = respawnPoint.position;
            body.velocity = Vector2.zero;
            fuelLevel = fuelMax;
            fuelSlider.value = fuelLevel;
            oxygenLevel = oxygenMax;
            oxygenSlider.value = oxygenLevel;
            playerVisual.SetActive(true);
            isAlive = true;
            StartCoroutine(Invisible(3));

        }

    }

    IEnumerator Invisible(float time)
    {
        isInvisible = true;
        spriteAnimator.SetBool("Invisible", true);
        yield return new WaitForSeconds(time);
        spriteAnimator.SetBool("Invisible", false);
        isInvisible = false;
    }

    public void makeInvisible(float time)
    {
        StartCoroutine(Invisible(time));
    }
    
    public void explode()
    {
        if(isAlive)
        {
            CinemachineShake.Instance.ShakeCamera(20f, 0.2f);
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
