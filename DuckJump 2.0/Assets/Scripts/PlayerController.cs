using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    private Rigidbody2D rigidBody;
    public GameObject gameManager;
    private Camera gameCamera;
    public Trajectory trajectoryController;
    public Transform groundCheck;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private Vector2 force;
    private Vector3 lastVelocity;

    private float horizontalInput;
    private float verticalInput;
    private float distance;
    public float pushForce = 4f;
    private bool isPulling = false;
    private float groundCheckRadius = 0.2f;
    private bool canControl;

    [HideInInspector] 
    public bool isGrounded;
    [SerializeField]
    private LayerMask groundLayer;

    private bool isFacingRight = true;
    public float maximumXForce = 1f;
    public float maximumYForce = 1f;
    public float controlDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        canControl = true;
        gameCamera = Camera.main;
        rigidBody = GetComponent<Rigidbody2D>();
        DisableRigidBody();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = rigidBody.velocity.x;
        verticalInput = rigidBody.velocity.y;
        lastVelocity = rigidBody.velocity;

        //this needs looked at, can still jump in mid air sometimes even if false
        if (canControl)
        {
            if (Input.GetMouseButtonDown(0) && isGrounded)
            {
                isPulling = true;
                OnDesktopPullStart();
            }

            if (Input.GetMouseButtonUp(0) && isGrounded)
            {
                isPulling = false;
                OnDesktopPullEnd();
            }
        }

        if (isPulling)
        {
            OnDesktopPull();
        }

        if (horizontalInput > 0 && !isFacingRight)
        {
            FlipGameObject();
        }

        if (horizontalInput < 0 && isFacingRight)
        {
            FlipGameObject();
        }

    }

    private void FixedUpdate()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Ground")
            {    
                isGrounded = true;
            }
        }
    }

    /* 
        Methods for desktop controller
    */

    void OnDesktopPullStart()
    {
        DisableRigidBody();
        startPoint = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        trajectoryController.Show();
    }

    void OnDesktopPull()
    {
        endPoint = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;

        // Might to to re-think this, should max X-Y values be set before defining force above ^^ ?
        if (force.x > maximumXForce) force.x = maximumXForce;
        if (force.y > maximumYForce) force.y = maximumYForce;

        trajectoryController.UpdateDots(player.transform.position, force);
        Debug.DrawLine(startPoint, endPoint);
    }

    void OnDesktopPullEnd()
    {
        isGrounded = false;
        ActivateRigidBody();
        Push(force);
        trajectoryController.Hide();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // Collision detection for side walls & bouncing mechanics if any
        if(collision.collider.tag == "Wall")
        {
            Debug.Log("Player hit wall");
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            // this needs looked at, feels like bouncing off is too fast
            rigidBody.velocity = direction * Mathf.Max(speed - 1f, 0f);
        }

    }

    // Delay between when player hits ground and slingshot is can be used again
    IEnumerator EnableControlDelay()
    {
        yield return new WaitForSeconds(controlDelay);
        isGrounded = true;
    }

    // Flipping game object to face movement direction
    private void FlipGameObject()
    {
        Vector3 currentScale = player.transform.localScale;
        currentScale.x *= -1;
        player.transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    // Add force when releasing pull
    private void Push(Vector2 force)
    {
        rigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    public void ActivateRigidBody()
    {
        rigidBody.isKinematic = false;
    }

    public void DisableRigidBody()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.isKinematic = true;
    }

}
