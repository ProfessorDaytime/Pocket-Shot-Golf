using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//https://www.youtube.com/watch?v=p8e4Kpl9b28   - Stopped 4 minutes in
public class Projection : MonoBehaviour
{
    
    private Scene simulationScene;
    private PhysicsScene physicsScene;

    private void Start()
    {
        CreatePhysicsScene();
    }

    void CreatePhysicsScene(){

        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));

        physicsScene = simulationScene.GetPhysicsScene();

        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")){
            var ghostBall = Instantiate(ball.gameObject, ball.transform.position, ball.transform.rotation);
            ghostBall.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostBall, simulationScene);
        }

        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall")){
            var ghostWall = Instantiate(wall.gameObject, wall.transform.position, wall.transform.rotation);
            ghostWall.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostWall, simulationScene);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
