using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class PlayerMovement : MonoBehaviour
{

    [Header("General")]
    public Rigidbody2D rb;
    //public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform flashLight;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;

    

    private Vector2 movement;
    private float movementSpeed;

    
    

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

    }

    private void HandlePlayerAnimation()
    {
        //To do, make sprites!
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
        float addAngle = 270;
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(flashLight.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + addAngle;
        flashLight.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void HandlePlayerMovement()
    {

        rb.MovePosition(rb.position + movement * movementSpeed * Time.deltaTime);
    }
}
