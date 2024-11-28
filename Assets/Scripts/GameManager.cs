using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum CurrentPlayer{
        Player1,
        Player2
    }

    CurrentPlayer currentPlayer;
    bool isWinningShotForPlayer1 = false;
    bool isWinningShotForPlayer2 = false;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForBallMovementToStop = false;
    bool willSwapPlayers = false;
    bool ballPocketed = false;
    bool isGameOver = false;

    [SerializeField] float shotTimer = 3f;
    private float currentTimer;
    [SerializeField] float movementThreshold;



    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI messageText;

    [SerializeField] GameObject restartButton;

    [SerializeField] Transform headPosition;

    [SerializeField] Camera cueStickCamera;
    [SerializeField] Camera overheadCamera;
    Camera currentCamera;


    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
        currentTimer = shotTimer;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(isWaitingForBallMovementToStop && !isGameOver){
            currentTimer -= Time.deltaTime;;
            if(currentTimer > 0){
                return;
            }

            //Logic here to check if balls are moving
            bool allStopped = true;
            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")){
                if(ball.GetComponent<Rigidbody>().velocity.magnitude >= movementThreshold){
                    Debug.Log(ball.GetComponent<Rigidbody>().velocity.magnitude);
                    allStopped = false;
                    break;
                }
            }
            
            //when all balls have stopped moving, it can move to the next player's turn
            if(allStopped){
                isWaitingForBallMovementToStop = false;
                if(willSwapPlayers || !ballPocketed){
                    NextPlayerTurn();
                } else{
                    SwitchCameras();
                }
                currentTimer = shotTimer;
                ballPocketed = false;
            }
        
        }
    }

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

            currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }

    public void RestartTheLevel(){
        SceneManager.LoadScene(1);
    }

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

        // NextPlayerTurn();
        // isWaitingForBallMovementToStop = true;
        willSwapPlayers = true;

        return false;
    }

    void EarlyEightBall(){
        if(currentPlayer == CurrentPlayer.Player1){
            Lose("Player 1 Hit in the Eight Ball Too Early and Has Lost!");
        } else{
            Lose("Player 2 Hit in the Eight Ball Too Early and Has Lost!");
        }

    }

    void ScratchOnWinningShot(string player){
        Lose(player + " Scratched on their final shot and has lost!");
    }

    // void NoMoreBalls(CurrentPlayer player){

    //     if (player == CurrentPlayer.Player1){
    //         isWinningShotForPlayer1 = true;
    //     } else{
    //         isWinningShotForPlayer2 = true;
    //     }

    // }

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
                    // NextPlayerTurn();
                    // isWaitingForBallMovementToStop = true;
                    willSwapPlayers = true;
                }
            } else{
                player2BallsRemaining --;
                player2BallsText.text = "Player 1 Balls Remaining: " + player2BallsRemaining;
                if(player2BallsRemaining <= 0){
                    isWinningShotForPlayer2 = true;
                }
                if(currentPlayer != CurrentPlayer.Player2){
                    // NextPlayerTurn();
                    // isWaitingForBallMovementToStop = true;
                    willSwapPlayers = true;
                }
            }
        }

        return true;
    }

    void Lose(string message){
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        restartButton.SetActive(true);
    }

    void Win(string player){
        isGameOver = true;
        messageText.gameObject.SetActive(true);
        messageText.text = player + " HAS WON!";
        restartButton.SetActive(true);
    }

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
