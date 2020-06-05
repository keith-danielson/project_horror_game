using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lockedSpeed = 2f;
    [SerializeField] float lightRotationSpeed = 5f;
    [SerializeField] float lockedLightRotationSpeed = 2f;

    public Rigidbody2D rb;
    //public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform flashLight;

    [SerializeField] float sprintSpeed = 10f;

    Vector2 movement;
    float movementSpeed;
    bool isLocked = false;
    
    

    // Update is called once per frame
    void Update()
    {
        TakePlayerInput();
        HandlePlayerSpeed();

        HandlePlayerAnimation();

    }

    private void TakePlayerInput()
    {
        //Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void HandlePlayerSpeed()
    {
        //Sprint input
        if (Input.GetAxisRaw("Sprint") > 0.1)
        {
            movementSpeed = sprintSpeed;

        }
        else
        {
            movementSpeed = moveSpeed;
        }
        if (Input.GetAxisRaw("Lock") > 0.1)
        {
            isLocked = true;
            movementSpeed = lockedSpeed;
        }
        else
        {
            isLocked = false;
        }
    }

    private void HandlePlayerAnimation()
    {
        //animator.SetFloat("Horizontal", movement.x);
        //animator.SetFloat("Vertical", movement.y);
        //animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        HandlePlayerMovement();
        HandleLightRotation();

    }

    private void HandleLightRotation()
    {
        if (movement != Vector2.zero)
        {
            if (isLocked == true)
            {

                flashLight.transform.rotation = Quaternion.Slerp(flashLight.transform.rotation, Quaternion.LookRotation(Vector3.forward, movement * -1), Time.deltaTime * lockedLightRotationSpeed);
            }
            else
            {
                flashLight.transform.rotation = Quaternion.Slerp(flashLight.transform.rotation, Quaternion.LookRotation(Vector3.forward, movement), Time.deltaTime * lightRotationSpeed);
            }

        }
    }

    private void HandlePlayerMovement()
    {
        //To do, make left sprites
        if (movement.x < -0.1)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }


        rb.MovePosition(rb.position + movement * movementSpeed * Time.deltaTime);
    }
}
