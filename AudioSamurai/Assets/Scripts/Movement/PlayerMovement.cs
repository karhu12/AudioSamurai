using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float bpm;
    public Rigidbody rb;
    private float speed = 10f;
    private bool onGround = true;
    private bool jumpKeyPressed = false;
    private bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        bpm /= 60f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Constantly observe if the player has pressed any buttons.
        CheckPlayerInput();
    }

    void FixedUpdate()
    {
        //Add direction and velocity to player character depending on a song bpm
        if (isRunning)
        {
            transform.position -= new Vector3(bpm * Time.deltaTime * 2, 0f, 0f);
        }
        //Check if character should jump 
        if (onGround && jumpKeyPressed)
        {
            float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

            transform.Translate(horizontal, 0, vertical);
            rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
            onGround = false;
        }
        jumpKeyPressed = false;

        //Slow down the velocity a little bit when jumping in order to make the physics more realistic.
        //***THIS COULD SET THE PACE OFF BEAT SO MAY NEED TO LOOK INTO THIS IN THE FUTURE. NEEDS SOME TESTING. MIGHT BE IRRELEVANT.***
        if (!onGround && isRunning)
        {
            transform.position += new Vector3(bpm * Time.deltaTime * 0.38f, 0f, 0f);
        }
    }

    //Check what possible keys the player can press
    void CheckPlayerInput()
    {
        //Default jump key is Space, can be changed from Unity input settings
        if (Input.GetButtonDown("Jump"))
        {
            jumpKeyPressed = true;
        }
        //Default submit key is Enter, can be changed from Unity input settings
        else if (Input.GetButtonDown("Submit"))
        {
            isRunning = !isRunning;
        }
    }

    //Check when character hits the ground to make the game logic reasonable. This way player can't jump while being mid-air.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }
}

