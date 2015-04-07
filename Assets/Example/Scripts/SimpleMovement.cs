using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour {


    void FixedUpdate()
    {
        if (Input.GetKeyDown("w"))
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 7f);
        Rigidbody2D rBody = GetComponent<Rigidbody2D>();
        if(Input.GetKey("a"))
            rBody.velocity = new Vector2(-2f, rBody.velocity.y);
        if (Input.GetKey("d"))
            rBody.velocity = new Vector2(2f , rBody.velocity.y);

    }
}
