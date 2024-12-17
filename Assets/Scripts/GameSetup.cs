using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    //-----------------------
    // Tracks the remaining red and blue balls to ensure correct placement logic
    //-----------------------
    // Total number of red balls to be placed
    private int redBallsRemaining = 7;
    // Total number of blue balls to be placed
    private int blueBallsRemaining = 7;

    // the radius of a ball calculated from the prefab's SphereCollider.
    private float ballRadius;
    // The diameter of a ball 
    private float ballDiameter;

    [Header("Setup Objects")]
    [Tooltip("Prefab used to instantiate balls in the game.")]
    [SerializeField] private GameObject ballPrefab;
    [Tooltip("Position where the cue ball will be placed")]
    [SerializeField] private Transform cueBallPosition;
    [Tooltip("Position where the first ball in the triangle starts")]
    [SerializeField] private Transform headBallPosition;


    /// <summary>
    ///  Called before Start - Calculates ball dimensions and places all balls on the table
    /// </summary>
    private void Awake(){
        // Calculate ball radius and diameter using the prefab's SphereCollider
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballDiameter = ballRadius * 2;

        
        // Call method to set up the table by placing balls
        PlaceAllBalls();
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        
    }


    /// <summary>
    /// Places all balls on the table, the cue ball and the random arrangement of colored balls
    /// </summary>
    void PlaceAllBalls(){
        // Place the cue ball at its designated position
        PlaceCueBall();
        // Place the red, blue and eight balls in a triangle arrangement on the table
        PlaceRandomBalls();
    }

    /// <summary>
    /// Instantiates and places the cue ball at the predefined position
    /// </summary>
    void PlaceCueBall(){
        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        // Mark the ball as the cue ball
        ball.GetComponent<Ball>().MakeCueBall();
    }


    /// <summary>
    /// Instantiates and places the eight ball at a specified position
    /// </summary>
    /// <param name="position">The position where the eight ball should be placed</param>
    void PlaceEightBall(Vector3 position){
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        // Mark the ball as the eight ball
        ball.GetComponent<Ball>().MakeEightBall();
    }


    /// <summary>
    /// Places red, blue, and the eight ball in a triangle arrangement starting from the head ball position
    /// </summary>
    void PlaceRandomBalls(){
        // Tracks the number of balls in the current row starting at 1
        int NumInThisRow = 1;
        // Random number to determine red or blue placement
        int rand;

        // Set the position of the first ball in the triangle
        Vector3 firstInRowPosition = headBallPosition.position;
        Vector3 currentPosition = firstInRowPosition;

        // Local function to place a red ball at the given position
        void PlaceRedBall(Vector3 position){
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            // Mark ball as red
            ball.GetComponent<Ball>().BallSetup(true);
            // Decrease the remaining count of red balls
            redBallsRemaining --;
        }

        // local function to place a blue ball at the given position
        void PlaceBlueBall(Vector3 position){
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            // mark ball as blue
            ball.GetComponent<Ball>().BallSetup(false);
            // decrease the remaing count of blue balls
            blueBallsRemaining --;
        }

        //Five Rows
        //Loop to create the trianglular ball arrangement
        for(int i = 0; i < 5; i++){
            // Loop to place balls in the current row
            for(int j = 0; j < NumInThisRow; j++){
                // Place the eight ball in the correct position, row 3, ball 2
                if(i == 2 && j == 1){
                    PlaceEightBall(currentPosition);
                } 
                // Place a random red or blue ball if both colors are still available
                else if(redBallsRemaining > 0 && blueBallsRemaining > 0){
                    // Randomly pick between red 0 and blue 1
                    rand = Random.Range(0,2);
                    if(rand == 0){
                        PlaceRedBall(currentPosition);
                    } else{
                        PlaceBlueBall(currentPosition);
                    }
                }

                // place only red balls if no blue balls are remaining
                else if(redBallsRemaining > 0){
                    PlaceRedBall(currentPosition);
                }
                // place only blue balls if no red balls are remaining
                else{
                    PlaceBlueBall(currentPosition);
                }

                // move the current position for the next ball in this row to the right
                currentPosition += new Vector3(1, 0, 0).normalized * ballDiameter;
            }

            //Once all of the balls in the row have been placed, move to the next row
            // Move to the next row in the triangle formation
            // - Downwards using Vector3.back for the new row
            // - Slightly to the left to center-align the row
            firstInRowPosition += Vector3.back * (Mathf.Sqrt(3) * ballRadius) + Vector3.left * ballRadius;

            // Reset the current position to the first ball of the new row
            currentPosition = firstInRowPosition;

            // Increase the number of balls in the next row
            NumInThisRow++;
        }
    }

}
