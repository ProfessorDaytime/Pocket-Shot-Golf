using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    private int redBallsRemaining = 7;
    private int blueBallsRemaining = 7;

    private float ballRadius;
    private float ballDiameter;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform cueBallPosition;
    [SerializeField] private Transform headBallPosition;


    //Awake is called before Start

    private void Awake(){
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballDiameter = ballRadius * 2;

        

        PlaceAllBalls();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void PlaceAllBalls(){
        PlaceCueBall();
        PlaceRandomBalls();
    }

    void PlaceCueBall(){
        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeCueBall();
    }

    void PlaceEightBall(Vector3 position){
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeEightBall();
    }

    void PlaceRandomBalls(){
        int NumInThisRow = 1;
        int rand;
        Vector3 firstInRowPosition = headBallPosition.position;
        Vector3 currentPosition = firstInRowPosition;

        void PlaceRedBall(Vector3 position){
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(true);
            redBallsRemaining --;
        }

        void PlaceBlueBall(Vector3 position){
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(false);
            blueBallsRemaining --;
        }

        //Five Rows
        for(int i = 0; i < 5; i++){
            //Balls in each row
            for(int j = 0; j < NumInThisRow; j++){
                //Check if it's where the eight ball goes
                if(i == 2 && j == 1){
                    PlaceEightBall(currentPosition);
                } 
                //check if rand and blue balls are remaining
                else if(redBallsRemaining > 0 && blueBallsRemaining > 0){
                    rand = Random.Range(0,2);
                    if(rand == 0){
                        PlaceRedBall(currentPosition);
                    } else{
                        PlaceBlueBall(currentPosition);
                    }

                }
                //if only red balls are remaining
                else if(redBallsRemaining > 0){
                    PlaceRedBall(currentPosition);
                }
                //if only blue balls
                else{
                    PlaceBlueBall(currentPosition);
                }

                // move the current position for the next ball in this row to the right
                currentPosition += new Vector3(1, 0, 0).normalized * ballDiameter;
            }

            //Once all of the balls in the row have been placed, move to the next row
            firstInRowPosition += Vector3.back * (Mathf.Sqrt(3) * ballRadius) + Vector3.left * ballRadius;
            currentPosition = firstInRowPosition;
            NumInThisRow++;
        }
    }






}
