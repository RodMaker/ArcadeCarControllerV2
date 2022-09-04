using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // get input
    // use input to move sphere

    public Rigidbody sphereRB;
    public Rigidbody carRB;

    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;
    
    public float alignToGroundTime;

    // Start is called before the first frame update
    void Start()
    {
        // detach sphere from car
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;

        normalDrag = sphereRB.drag;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");

        // calculate turning rotation
        float newRotation = turnInput * turnSpeed * Time.deltaTime * moveInput;
        
        if (isCarGrounded)
            transform.Rotate(0, newRotation, 0, Space.World);

        // set cars position to sphere
        transform.position = sphereRB.transform.position;

        // raycast ground check
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        // rotate car to allign with ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        // calculate movement direction
        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        // calculate drag
        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
        {
            // move car
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            // add extra gravity
            sphereRB.AddForce(transform.up * -40f);
        }
        carRB.MoveRotation(transform.rotation);
    }
}
