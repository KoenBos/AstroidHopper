using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    public float moveSpeed, maxSpeed, jumpPower;
    private bool isGrounded;
    private float horizontal;
    [SerializeField] private ParticleSystem particleEmitter;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            body.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if(isGrounded)
        {
            body.AddForce(transform.right * horizontal * moveSpeed, ForceMode2D.Force);
            body.AddForce(-transform.up * 10);
            StopParticles();
        }
        else
        { //rotate player if not on the ground
            transform.Rotate(Vector3.forward * -horizontal * 3);

    

            if (Input.GetKey(KeyCode.Space) && body.velocity.magnitude < maxSpeed)
            {
                body.AddForce(transform.up * moveSpeed * Time.deltaTime * 50);
                StartParticles();
            }
            else
            {
                StopParticles();
            }
        }
    }
//-------------------------------------------------
    void OnTriggerStay2D(Collider2D obj)
    {
       if (obj.CompareTag("Planet"))
       {
            body.drag = 0.4f;

            float distance = Mathf.Abs(obj.GetComponent<GravityPoint>().planetRadius - Vector2.Distance(transform.position, obj.transform.position));
            if (distance < 0.6f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
       }
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.CompareTag("Planet"))
        {
            body.drag = 0.1f;
        }
    }

    void StopParticles()// stop loop and stop particles
    {
        particleEmitter.loop = false; 
        if (particleEmitter.isPlaying)
        {
            particleEmitter.Stop();
        }
    }
    void StartParticles()// start loop and start particles
    {
        if (!particleEmitter.isPlaying)
        {
            particleEmitter.Play();
        }
        particleEmitter.loop = true;
    }
}
