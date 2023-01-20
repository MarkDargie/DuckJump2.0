using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public SlingshotDesktopController slingshotController;
    private Camera gameCamera;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private Vector2 force;
    private float distance;
    public float pushForce = 4f;
    private bool isPulling = false;
    [HideInInspector] public bool isGrounded; // add a small delay before being able to jump again
    public float maximumXForce = 1f;
    public float maximumYForce = 1f;
    public float controlDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        gameCamera = Camera.main;
        slingshotController.DisableRigidBody();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Mouse button down");
                isPulling = true;
                OnDesktopPullStart();
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("Mouse button up");
                isPulling = false;
                OnDesktopPullEnd();
            }

            if (isPulling)
            {
                //Debug.Log("isPulling = true");
                OnDesktopPull();
            }
        }

    }

    /* 
        Methods for desktop controller
    */

    void OnDesktopPullStart()
    {
        slingshotController.DisableRigidBody();
        startPoint = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log($"OnDesktopPullStart(): Setting start point: {startPoint}");
    }

    void OnDesktopPull()
    {
        endPoint = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;

        if (force.x > maximumXForce) force.x = maximumXForce;
        if (force.y > maximumYForce) force.y = maximumYForce;

        Debug.Log($"Adding Force: {force}");
        //Debug.Log($"OnDesktopPull(): setting endpoint: {endPoint}, distance: {distance}, direction: {direction}, force: {force}");
        Debug.DrawLine(startPoint, endPoint);
    }

    void OnDesktopPullEnd()
    {
        slingshotController.ActivateRigidBody();
        slingshotController.Push(force);
        //Debug.Log($"OnDesktopPullEnd(): Adding force of X:{force.x.ToString()} - Y:{force.y.ToString()}");
    }


    /* 
        Methods for mobile controller
    */

    void OnMobilePullStart()
    {
    }

    void OnMobilePull()
    {
    }

    void OnMobilePullEnd()
    {
    }

    // Play around with having this on the player objects, or a separate ground checker child object
    // Not sure how i want to do this yet, will come back to this.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GroundTile")
        {
            Debug.Log("Player isGrounded Triggered");
            StartCoroutine(EnableControlDelay());
        }
    }

    // Delay between when player hits ground and slingshot is can be used again
    IEnumerator EnableControlDelay()
    {
        yield return new WaitForSeconds(controlDelay);
        isGrounded = true;
    }
}
