using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float jumpPower, walkSpeed, maxWalkSpeed, rotateSpeed, flySpeed;
    private bool isGrounded, longGrounded, activated;
    private bool outsideGravity = true;
    private float horizontal;
    private bool isAlive = true;
    private GameObject player;
    


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.Find("Player");
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
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        if(!activated)
        {
            if (player != null)
            {
                if (Vector2.Distance(transform.position, player.transform.position) < 10)
                {
                    activated = true;
                    walkSpeed *= 2;
                }
            }
        }
        else if (Vector2.Distance(transform.position, player.transform.position) > 20)
        {
            jumpPower = 8;
        }
        else
        {
            jumpPower = 5;
        }


        //random jump en move
        if (isAlive)
        {
            if (!outsideGravity)
            {
                if (activated)
                {
                    if (longGrounded && Random.Range(0, 500) == 1)
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
                if (activated)
                {
                    //Uit de gravity draai naar player en vlieg naar player
                    if (player != null)
                    {
                        //slowly rotate z axis with rotatespeed and deltatime 2d facing the player with a lerp - 90 degrees to make it face the player correctly
                        transform.up = Vector3.Lerp(transform.up, (player.transform.position - transform.position).normalized, rotateSpeed * Time.deltaTime);

                        //move towards the player
                        body.AddForce(transform.up * flySpeed * Time.deltaTime);
                    }  
                }

            }
        }
    }
    //botsen met alien = omdraaien
    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.CompareTag("Alien"))
        {
            horizontal *= -1;
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
                if (body.linearVelocity.magnitude < maxWalkSpeed)
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
