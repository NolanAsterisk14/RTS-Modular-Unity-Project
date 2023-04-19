using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject camera;
    private Transform camTransform;
    /* the following are series of options for the developer to choose how the camera starts
     * and how the camera is controlled in play mode */
    [SerializeField]
    [Tooltip("Speed camera moves using wasd keys")]
    private float speed = 10;
    [SerializeField]
    [Tooltip("Speed camera moves using drag movement")]
    private float dragSpeed = 20;
    [SerializeField]
    [Tooltip("Speed camera moves using edge movement")]
    private float edgeSpeed = 20;
    [SerializeField]
    [Tooltip("How far from the edge of the screen the cursor will move the camera (0.01-0.99)")]
    private float edgeBounds = 0.15f;
    [SerializeField]
    [Tooltip("Camera starting angle")]
    private float angle = 20;
    [SerializeField]
    [Tooltip("Camera starting height")]
    private float height = 15;
    [SerializeField]
    [Tooltip("Should Camera be moved with wasd keys?")]
    private bool keyControl = true;
    [SerializeField]
    [Tooltip("Should Camera be moved by middle mouse drag?")]
    private bool dragControl = true;
    [SerializeField]
    [Tooltip("Should Camera be moved by cursor approaching screen edges?")]
    private bool edgeControl = true;
    // various variables needed for calculations
    [SerializeField]
    private Vector2 mousePos;
    [SerializeField]
    private Vector2 mouseCoord;
    
    
    void Start()
    {   //store values for camera
        camera = this.gameObject;
        camTransform = this.gameObject.transform;
        camTransform.rotation = Quaternion.Euler(angle, 0, 0);
        camTransform.position = new Vector3(0, height, 0);
    }

    void Update()
    {   //store mouse position
        mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //store raw mouse coordinates
        mouseCoord = Input.mousePosition;
        //control camera with keys
        if (keyControl == true)
        {
            if (Input.GetKey("w") == true)
            {
                camTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("a") == true)
            {
                camTransform.Translate(Vector3.left * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("d") == true)
            {
                camTransform.Translate(Vector3.right * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("s") == true)
            {
                camTransform.Translate(Vector3.back * Time.deltaTime * speed, Space.World);
            }
        }
        //control camera with middle mouse drag
        if (dragControl == true)
        {
            if (Input.GetKey("mouse 2") == true)
            {
                camTransform.position -= new Vector3(mousePos.x * Time.deltaTime * dragSpeed * 20, 0, mousePos.y * Time.deltaTime * dragSpeed * 20);
            }
        }
        //control camera with cursor approaching edges
        if (edgeControl == true)
        {
            if (mouseCoord.x < (Screen.width * 0.5 * edgeBounds))
            {
                camTransform.Translate(Vector3.left * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.x > (Screen.width * 0.5 * (2 - edgeBounds)))
            {
                camTransform.Translate(Vector3.right * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.y < (Screen.height * 0.5 * edgeBounds))
            {
                camTransform.Translate(Vector3.back * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.y > (Screen.height * 0.5 * (2 - edgeBounds)))
            {
                camTransform.Translate(Vector3.forward * Time.deltaTime * edgeSpeed, Space.World);
            }
        }
    }

}
