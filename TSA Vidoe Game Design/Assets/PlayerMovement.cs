using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;


    //private Animator anim;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();

    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;

       // anim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
      // anim.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));

        
    }

    void FixedUpdate()
    {

        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        //transform.LookAt(moveVelocity);


    }
}
