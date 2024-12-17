using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the player's camera, allowing it to rotate around the cue ball and handle shooting mechanics
/// </summary>

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Speed at which the camera rotates around the cue ball.")]
    [SerializeField] float rotationSpeed;
    [Tooltip("Offset position of the camera relative to the cue ball.")]
    [SerializeField] Vector3 offset;

    [Tooltip("The downward angle of the camera when reset.")]
    [SerializeField] float downAngle;

    [Header("Shooting Settings")]
    [Tooltip("Base power multiplier for the shot.")]
    [SerializeField] float power;

    [Tooltip("Maximum distance the cue can be drawn back for a shot.")]
    [SerializeField] float maxDrawDistance;

    [Header("References")]
    [Tooltip("Reference to the cue stick object.")]
    [SerializeField] GameObject cueStick;

    [Tooltip("UI text displaying shot power.")]
    [SerializeField] TextMeshProUGUI powerText;


    private float horizontalInput;
    private bool isTakingShot = false;
    private float savedMousePosition;
    private Transform cueBall;
    private GameManager gameManager;


    /// <summary>
    /// Initializes references and finds the cue ball in the scene.
    /// </summary>
    void Start()
    {
        // Get reference to the GameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Find the cue ball by iterating through all objects tagged as "Ball"
        foreach(GameObject ball in GameObject.FindGameObjectsWithTag("Ball")){
            if(ball.GetComponent<Ball>().IsCueBall()){
                cueBall = ball.transform;
                break;
            }
        }
    }

    /// <summary>
    /// Updates camera rotation and handles shooting logic.
    /// </summary>
    void Update()
    {
        // Rotate camera around the cue ball if not taking a shot
        if(cueBall != null && !isTakingShot){
            horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.RotateAround(cueBall.position, Vector3.up, horizontalInput);
        }

        // Handle shooting input
        Shoot();
    }

    /// <summary>
    /// Resets the camera position and orientation to the default position relative to the cue ball.
    /// </summary>
    public void ResetCamera(){
        cueStick.SetActive(true);
        // gameManager.SwitchCameras();
        transform.position = cueBall.position + offset;
        transform.LookAt(cueBall.position);
        transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0);

    }


    /// <summary>
    /// Handles the shooting mechanics, including power buildup and firing the cue ball.
    /// </summary>
    void Shoot(){
        if(gameObject.GetComponent<Camera>().enabled){

            // Start charging a shot
            if(Input.GetButtonDown("Fire1") && !isTakingShot){
                isTakingShot = true;
                savedMousePosition = 0f;

            } else if(isTakingShot){

                // Charge the shot power
                if(savedMousePosition + Input.GetAxis("Mouse Y") <= 0){
                    savedMousePosition += Input.GetAxis("Mouse Y");

                    // savedMousePosition = Mathf.Clamp(savedMousePosition, -maxDrawDistance, 0);
                    if(savedMousePosition <= maxDrawDistance){
                        savedMousePosition = maxDrawDistance;
                    }

                    //Calculate and display power percentage
                    float powerValueNumber = ((savedMousePosition - 0) / (maxDrawDistance - 0) * (100 - 0) + 0);
                    int powerValueInt = Mathf.RoundToInt(powerValueNumber);
                    Debug.Log(powerValueInt);
                    powerText.text = ("Power: " + powerValueInt.ToString() + "%");
                }

                // Fire the shot
                if(Input.GetButtonDown("Fire1")){
                    Vector3 hitDirection = transform.forward;
                    hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;

                    cueBall.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * power * Mathf.Abs(savedMousePosition), ForceMode.Impulse);
                    
                    cueStick.SetActive(false);
                    gameManager.SwitchCameras();
                    isTakingShot = false;
                }
            }
        }
    }
}
