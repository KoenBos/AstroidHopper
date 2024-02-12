using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    public float moveSpeed, jumpPower;
    private bool isGrounded;
    private float horizontal;
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
        body.AddForce(transform.right * horizontal * moveSpeed);
    }
//-------------------------------------------------
    void OnTriggerStay2D(Collider2D obj)
    {
       if (obj.CompareTag("Planet"))
       {
            body.drag = 1f;

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
            body.drag = 0.2f;
        }
    }
}
