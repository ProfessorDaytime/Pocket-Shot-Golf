using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Runs the game tracking functions
/// </summary>
public class GameManager : MonoBehaviour
{
    
    // Enum to track the current player's turn
    enum CurrentPlayer{
        Player1,
        Player2
    }

    // Stores which player's turn it is
    CurrentPlayer currentPlayer;

    // Tracks if Player 1 is set for the winning shot
    bool isWinningShotForPlayer1 = false;

    // Tracks if Player 2 is set for the winning shot
    bool isWinningShotForPlayer2 = false;

    // Number of balls remaining for Player 1
    int player1BallsRemaining = 7;

    // Number of balls remaining for Player 2
    int player2BallsRemaining = 7;

    // Tracks whether the game is waiting for all balls to stop moving
    bool isWaitingForBallMovementToStop = false;

    // Indicates whether players will swap turns
    bool willSwapPlayers = false;
    
    // Tracks if a ball was pocketed during the turn
    bool ballPocketed = false;

    // Indicates whether the game has ended
    bool isGameOver = false;

    [Header("Turn End Settings")]
    [Tooltip("Time to wait for the ball movement to stop")]
    [SerializeField] float shotTimer = 3f;
    [Tooltip("Minimum movement threshold to consider a ball stopped")]
    [SerializeField] float movementThreshold;

    // Countdown timer for ball movement
    private float currentTimer;



    [Tooltip("UI element for Player 1's remaining balls")]
    [SerializeField] TextMeshProUGUI player1BallsText;

    [Tooltip("UI element for Player 2's remaining balls")]
    [SerializeField] TextMeshProUGUI player2BallsText;

    [Tooltip("UI element for displaying the current player's turn")]
    [SerializeField] TextMeshProUGUI currentTurnText;

    [Tooltip("UI element for displaying game messages")]
    [SerializeField] TextMeshProUGUI messageText;

    [Tooltip("Restart button object")]
    [SerializeField] GameObject restartButton;

    [Tooltip("Position for resetting balls")]
    [SerializeField] Transform headPosition;

    [Tooltip("Cue stick camera")]
    [SerializeField] Camera cueStickCamera;
    [Tooltip("Overhead camera")]
    [SerializeField] Camera overheadCamera;

    
    // Tracks the currently active camera
    Camera currentCamera;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
        currentTimer = shotTimer;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        
        // Handles waiting for all balls to stop moving before proceeding
        if(isWaitingForBallMovementToStop && !isGameOver){
            // Decrease timer
            currentTimer -= Time.deltaTime;;
            if(currentTimer > 0){
                return;
            }

            // Speed up time for ball movement check
            Time.timeScale = 3f;

            // Check if all balls have stopped moving
            bool allStopped = true;
            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")){
                if(ball.GetComponent<Rigidbody>().velocity.magnitude >= movementThreshold){
                    Debug.Log(ball.GetComponent<Rigidbody>().velocity.magnitude);
                    allStopped = false;
                    break;
                }
            }
            
            // If all balls have stopped, proceed with the next step
            if(allStopped){

                // Reset time scale
                Time.timeScale = 1f;

                isWaitingForBallMovementToStop = false;
                if(willSwapPlayers || !ballPocketed){
                    // Switch turns
                    NextPlayerTurn();
                } else{
                    // Stay with the same player
                    SwitchCameras();
                }
                // Reset timer
                currentTimer = shotTimer;
                ballPocketed = false;
            }
        
        }
    }

    /// <summary>
    /// Switches between the cue stick and overhead cameras
    /// </summary>
    public void SwitchCameras(){
        if(currentCamera == cueStickCamera){
            cueStickCamera.enabled = false;
            overheadCamera.enabled = true;
            currentCamera = overheadCamera;
            isWaitingForBallMovementToStop = true;
        } else{
            overheadCamera.enabled = false;
            cueStickCamera.enabled = true;
            currentCamera = cueStickCamera;

            // Reset camera position to follow the cue ball
            currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }

    /// <summary>
    /// Restarts the game level
    /// </summary>
    public void RestartTheLevel(){
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Handles scratching scenarios
    /// </summary>
    /// <returns></returns>
    bool Scratch(){
        if(currentPlayer == CurrentPlayer.Player1){
            if(isWinningShotForPlayer1){
                ScratchOnWinningShot("Player 1");
                return true;
            }
            
        } else{
            if(isWinningShotForPlayer2){
                ScratchOnWinningShot("Player 2");
                return true;
            }
        }

        //Force player swap
        willSwapPlayers = true;

        return false;
    }

    /// <summary>
    /// Handles Early eight ball scenarios
    /// </summary>
    void EarlyEightBall(){
        if(currentPlayer == CurrentPlayer.Player1){
            Lose("Player 1 Hit in the Eight Ball Too Early and Has Lost!");
        } else{
            Lose("Player 2 Hit in the Eight Ball Too Early and Has Lost!");
        }

    }

    /// <summary>
    /// Handles scratching on a winning shot
    /// </summary>
    /// <param name="player">Planer Name</param>
    void ScratchOnWinningShot(string player){
        Lose(player + " Scratched on their final shot and has lost!");
    }

   
    /// <summary>
    /// Checks which ball was pocketed and handles its logic
    /// </summary>
    /// <param name="ball">ball that's checked</param>
    /// <returns>if it is a ball or not</returns>
    bool CheckBall(Ball ball){
        if(ball.IsCueBall()){

            if(Scratch()){
                return true;
            } else {
                return false;
            }

        } else if (ball.IsEightBall()){

            if(currentPlayer == CurrentPlayer.Player1){
                if(isWinningShotForPlayer1) {
                    Win("Player 1");
                    return true;
                }
            } else{
                if(isWinningShotForPlayer2){
                    Win("Player 2");
                    return true;
                }
            }
            EarlyEightBall();
        } else{

            //All logic when not eight ball or cue ball
            if(ball.IsBallRed()){
                player1BallsRemaining --;
                player1BallsText.text = "Player 1 Balls Remaining: " + player1BallsRemaining;
                if(player1BallsRemaining <=0){
                    isWinningShotForPlayer1 = true;
                }
                if(currentPlayer != CurrentPlayer.Player1){
                    
                    willSwapPlayers = true;
                }
            } else{
                player2BallsRemaining --;
                player2BallsText.text = "Player 2 Balls Remaining: " + player2BallsRemaining;
                if(player2BallsRemaining <= 0){
                    isWinningShotForPlayer2 = true;
                }
                if(currentPlayer != CurrentPlayer.Player2){
                    
                    willSwapPlayers = true;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Handles losing the game
    /// </summary>
    /// <param name="message">Message explaining why what player lost</param>
    void Lose(string message){
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }

    /// <summary>
    /// Handles winning the game
    /// </summary>
    /// <param name="player">Which player won?</param>
    void Win(string player){
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = player + " HAS WON!";
        restartButton.SetActive(true);
    }

    /// <summary>
    /// Advances to the next player's turn
    /// </summary>
    void NextPlayerTurn(){
        if (currentPlayer == CurrentPlayer.Player1){
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Current Turn: Player 2";
        } else{
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Current Turn: Player 1";
        }
        willSwapPlayers = false;
        SwitchCameras();
    }

    /// <summary>
    /// Triggered when the ball enters a pocket
    /// </summary>
    /// <param name="other">was it a ball that went in the pocket</param>
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Ball"){
            ballPocketed = true;
            if(CheckBall(other.gameObject.GetComponent<Ball>())){
                Destroy(other.gameObject);
            } else{
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            }
        }
    }

}
