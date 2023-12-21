using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    float rightleft;
    float frontback;

    Vector3 move_dir;
    Transform playerfootpos; //creating a container to store where the foot of the player is (cause transform.pos is acting 
                             // stupid and not giving the actual pos for some reason) to make raycast usable

    Rigidbody rb;

    [Header("character info")]
    public float playerheight;


    [Header("movement")]
    public float movespeed;
    public float sprintspeed;
    public float walkspeed;
    public Transform playerorientation;
    public float ground_drag;
    public float airmovemult;
    private float tempspeed;
    private float tempgrddrag;

    [Header("ground check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask whatIsGround;
    bool grounded;
    public Vector3 boxSize;


    [Header("jump")]
    public float jumpstreagth;
    public float jumpcool;
    bool jumprdy;

    [Header("crouching")]
    public float crouchyscale;
    public float normyscale;
    public float crouchspeed;
    private bool crouchsaftey;

    [Header("keybinds")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode sprintkey = KeyCode.Q;
    public KeyCode crouchkey = KeyCode.C;

    [Header("slope handling")]
    public float maxclimbangle;
    public RaycastHit slopehit;
    public RaycastHit hit;
    private float slopeangle;


    public Movementstate state;
    public enum Movementstate
    {
        walking,
        sprinting,
        crouch,
        sliding,
        air,
    }

    public void statecheck()
    {



        if ((grounded && Input.GetKey(sprintkey)) && transform.localScale.y == normyscale)
        {
            state = Movementstate.sprinting;
            movespeed = sprintspeed;
        }

        else if (transform.localScale.y < normyscale)
        {
            state = Movementstate.crouch;
            movespeed = crouchspeed;
        }

        else if (!grounded)
        {
            state = Movementstate.air;
        }

        else if(grounded)
        {
            state = Movementstate.walking;
            movespeed = walkspeed;
        }



    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        jumprdy= true;

        normyscale = transform.localScale.y;

    }

    // Update is called once per frame
    void Update()
    {

        // ground check
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        playerfootpos = transform.Find("ground detector"); //updating value of player foot 
        myinput();

        // handle drag
        if (grounded)
            rb.drag = ground_drag;
        else
            rb.drag = 0;

        speedclamp();
        moveplayer();
        statecheck();



    }

    private void FixedUpdate()
    {
        //will be called at a fixed framerate
        Debug.DrawRay(playerfootpos.position, Vector3.down, Color.magenta);
        //Debug.Log(slopecheck());




    }



    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// 


    private void myinput()
    {
        //vertical is defined as w for +ve and s for -ve input in unity itself
        rightleft = Input.GetAxisRaw("Horizontal");
        frontback = Input.GetAxisRaw("Vertical");


        if (Input.GetKey(jumpkey) && jumprdy && state != Movementstate.air)
        {
            jumprdy = false;

            jump();

            Invoke(nameof(resetjump), jumpcool);
        }

        if (Input.GetKeyDown(crouchkey) && rb.velocity.magnitude < 1) 
        {
            
            transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, crouchyscale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouchsaftey = true;
        }

        if (Input.GetKeyUp(crouchkey) && crouchsaftey == true) 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 2.1f, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, normyscale, transform.localScale.z);
            crouchsaftey = false;
        }
    }
    private void moveplayer()
    {
        //calculate amount of force to apply and normalize it against frame rate
        move_dir = playerorientation.forward * frontback + playerorientation.right * rightleft;

        if (slopecheck())
        {
            rb.AddForce(slopemovedir() * movespeed * 10f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 50f, ForceMode.Force);
            }
        }

        else if(grounded)
            rb.AddForce(move_dir.normalized * movespeed * 10f, ForceMode.Force);
        //we normalize the move dir vec so as to not get a random mangitude spike in the speed then we multiply the assigned movement
        //speed of the object with a set constant (f indicates its a float value)
        //forcemode.force indicates force is to be applied unless keypress is released
        //forcemode.impulse will apply force only once per keypress or refresh event

        else if(!grounded)
            rb.AddForce(move_dir.normalized * movespeed * 10f * airmovemult , ForceMode.Force);

        rb.useGravity = !slopecheck();
    }

    private void speedclamp()
    {
        Vector3 flatvel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);



        if (flatvel.magnitude > movespeed)
        {
            Vector3 speedlimvec = flatvel.normalized * movespeed;
            rb.velocity = new Vector3(speedlimvec.x, rb.velocity.y, speedlimvec.z);
        }

    }

    private void jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f ,rb.velocity.z);
        rb.AddForce (transform.up * jumpstreagth, ForceMode.Impulse);
    }


    private void resetjump()
    {
        jumprdy= true;
    }

    private bool slopecheck()
    {
        if (Physics.Raycast(playerfootpos.position , Vector3.down, out slopehit, (playerheight * 0.5f) + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopehit.normal);
            slopeangle = angle;
            return angle < maxclimbangle && angle > 0;
        }
        return false;
    }

    private Vector3 slopemovedir()
    {
        return Vector3.ProjectOnPlane(move_dir, slopehit.normal).normalized;
    }
}
