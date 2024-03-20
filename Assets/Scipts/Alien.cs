using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float jumpPower, walkSpeed, maxWalkSpeed, rotateSpeed, flySpeed;
    private bool isGrounded, longGrounded, outsideGravity;
    private float horizontal;
    private bool isAlive = true;
    


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Jump()
    {
        if (isGrounded)
        {
            body.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void Update()
    {
        //randomly jump and move left or right
        if (isAlive)
        {
            if (!outsideGravity)
            {
                if (isGrounded)
                {
                    if (Random.Range(0, 500) == 1)
                    {
                        Jump();
                    }
                }
                if (Random.Range(0, 1000) == 64)
                {
                    horizontal = 1;
                }
                if (Random.Range(0, 1000) == 128)
                {
                    horizontal = -1;
                }
            }
            else
            {
                //if outside gravity, rotate the alien so it faces the player
                GameObject player = GameObject.Find("Player");
                if (player != null)
                {
                    Vector3 direction = player.transform.position - transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                    //move towards the player
                    body.AddForce(transform.up * flySpeed * Time.deltaTime);

                }  
            }
        }
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
                if (body.velocity.magnitude < maxWalkSpeed)
                {
                    body.AddForce(transform.right * horizontal * walkSpeed, ForceMode2D.Force);
                }
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
    }

    void OnTriggerStay2D(Collider2D obj) //inside planet gravity
    {
        if (obj.CompareTag("Planet"))
        {
            outsideGravity = false;

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
            outsideGravity = true;
            horizontal = 0;
        }
    }

    IEnumerator Die()
    {
        isAlive = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    
    public void goDie()
    {
        if(isAlive)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator LongGrounded() //lang op de grond voor fuel tanken
    {
        yield return new WaitForSeconds(0.5f);
        longGrounded = true;
    }
}
