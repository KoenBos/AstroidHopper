using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    public float gravityScale, planetRadius, gravityMinRange, gravityMaxRange;
    // Start is called before the first frame update
    //ps mini blastoff
    //https://www.youtube.com/watch?v=sK8FrL3ZJi0
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        float gravitationalPower = gravityScale;
        float dist = Vector2.Distance(obj.transform.position, transform.position);

        if (dist > (planetRadius + gravityMinRange))
        {
            float min = planetRadius + gravityMinRange;
            gravitationalPower = (((min + gravityMaxRange) - dist) / gravityMaxRange) * gravityScale;
        }
        Vector2 dir  = (transform.position - obj.transform.position) * gravityScale;
        obj.GetComponent<Rigidbody2D>().AddForce(dir);


        //Draait de player naar de planeet
        if (obj.CompareTag("Player"))
        {
            obj.transform.up = Vector3.MoveTowards(obj.transform.up, -dir, planetRadius * Time.deltaTime);
        }
    } 
}
