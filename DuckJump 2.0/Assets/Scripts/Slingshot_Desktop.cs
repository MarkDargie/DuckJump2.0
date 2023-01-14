using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot_Desktop : MonoBehaviour
{

    public float power = 10f;
    public Rigidbody2D rb;

    public Vector2 minPower;
    public Vector2 maxPower;

    private Camera cam;

    private Vector2 force;
    private Vector3 startPoint;
    private Vector3 endPoint;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            startPoint.z = 15;
            Debug.Log("Start Point: " + startPoint.ToString());
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            endPoint.z = 15;
            Debug.Log("End Point: " + startPoint.ToString());

            force = new Vector2(Mathf.Clamp(startPoint.x - endPoint.x, minPower.x, maxPower.x), Mathf.Clamp(startPoint.y - endPoint.x, minPower.y, maxPower.y));
            rb.AddForce(force * power, ForceMode2D.Impulse); // adds instant force

        }
    }
}
