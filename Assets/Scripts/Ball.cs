using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour{

    public float moveSpeed = 12.0f;

    public Vector2 ballDirection = Vector2.left;

    public float topBounds = 9.4f;
    public float bottomBounds = -9.4f;


    private float playerPaddleHeight, playerPaddleWidth, computerPaddleHeight, computerPaddleWidth,playerPaddleMaxX,playerPaddleMaxY,
        playerPaddleMinX,playerPaddleMinY,computerPaddleMaxX,computerPaddleMaxY,computerPaddleMinX,computerPaddleMinY,ballWidth,ballHeight;

    private GameObject paddlePlayer, paddleComputer;

    private float bounceAngle; 
    private float vx, vy; 
    private float maxAngle = 45.0f;
    private bool collidedWithPlayer, collidedWithWall, collidedWithComputer;

    private Game game;
    private bool assignedpoint;



    void Start(){

        game = GameObject.Find("Game").GetComponent<Game>();

        if (moveSpeed < 0)
            moveSpeed = -1 * moveSpeed;

        paddlePlayer = GameObject.Find("player_paddle");
        paddleComputer = GameObject.Find("computer_paddle");

        playerPaddleHeight = paddlePlayer.transform.GetComponent<SpriteRenderer>().bounds.size.y;
        playerPaddleWidth = paddlePlayer.transform.GetComponent<SpriteRenderer>().bounds.size.x;
        computerPaddleHeight = paddleComputer.transform.GetComponent<SpriteRenderer>().bounds.size.y;
        computerPaddleWidth = paddleComputer.transform.GetComponent<SpriteRenderer>().bounds.size.x;
        ballHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y;
        ballWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x;

        playerPaddleMaxX = paddlePlayer.transform.localPosition.x + playerPaddleWidth / 1.8f;// because x is the center
        playerPaddleMinX = paddlePlayer.transform.localPosition.x - playerPaddleWidth / 1.8f;

        computerPaddleMaxX = paddleComputer.transform.localPosition.x - computerPaddleWidth / 1.8f;
        computerPaddleMinX = paddleComputer.transform.localPosition.x + computerPaddleWidth / 1.8f;

        bounceAngle = GetRandomBounceAngle();

        vx = moveSpeed * Mathf.Cos(bounceAngle);
        vy = moveSpeed * -Mathf.Sin(bounceAngle);

    }

    void Update(){
        if(game.gameState != Game.GameState.Paused)
        {
            Move();
        }
    }

    bool CheckCollision(){

        playerPaddleMaxY = paddlePlayer.transform.localPosition.y + playerPaddleHeight / 1.8f;
        playerPaddleMinY = paddlePlayer.transform.localPosition.y - playerPaddleHeight / 1.8f;

        computerPaddleMaxY = paddleComputer.transform.localPosition.y + computerPaddleHeight / 1.8f;
        computerPaddleMinY = paddleComputer.transform.localPosition.y - computerPaddleHeight / 1.8f;

        if(transform.localPosition.x - ballWidth / 1.8f < playerPaddleMaxX && transform.localPosition.x + ballWidth / 1.8f > playerPaddleMinX){

            if (transform.localPosition.y - ballHeight / 1.8f < playerPaddleMaxY && transform.localPosition.y + ballHeight / 1.8f > playerPaddleMinY){

                ballDirection = Vector2.right;
                collidedWithPlayer = true;
                transform.localPosition = new Vector3(playerPaddleMaxX + ballWidth / 1.8f, transform.localPosition.y,transform.localPosition.z);
                return true;

            }else{
                if (!assignedpoint){
                    assignedpoint = true; 
                    game.ComputerPoint();
                }
            }
        }

        if (transform.localPosition.x + ballWidth / 1.8f > computerPaddleMaxX && transform.localPosition.x - ballWidth / 1.8f < computerPaddleMinX)
        {
            if (transform.localPosition.y - ballHeight / 1.8f < computerPaddleMaxY && transform.localPosition.y + ballHeight / 1.8f > computerPaddleMinY)
            {
                ballDirection = Vector2.left;
                collidedWithComputer = true;
                transform.localPosition = new Vector3(computerPaddleMaxX - ballWidth / 1.8f, transform.localPosition.y, transform.localPosition.z);
                return true;
            }else{
                if (!assignedpoint){
                    assignedpoint = true;
                    game.PlayerPoint();
                }
            }
        }

        if(transform.localPosition.y > topBounds){

            transform.localPosition = new Vector3(transform.localPosition.x, topBounds, transform.localPosition.z);
            collidedWithWall = true;
            return true;
        }
        if(transform.localPosition.y < bottomBounds)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, bottomBounds, transform.localPosition.z);
            collidedWithWall = true;
            return true;

        }
        return false;
    }

    void Move(){
        if (!CheckCollision())
        {

            vx = moveSpeed * Mathf.Cos(bounceAngle);

            if (moveSpeed > 0)
                vy = moveSpeed * -Mathf.Sin(bounceAngle);
            else
                vy = moveSpeed * Mathf.Sin(bounceAngle);

            transform.localPosition += new Vector3(ballDirection.x * vx * Time.deltaTime, vy * Time.deltaTime, 0);

        }
        else
        {

            if (moveSpeed < 0)
                moveSpeed = -1 * moveSpeed;
            if (collidedWithPlayer)
            {
                collidedWithPlayer = false;
                float relativeIntersectY = paddlePlayer.transform.localPosition.y - transform.localPosition.y;
                float normalizeRelativeIntersectionY = (relativeIntersectY / (playerPaddleHeight / 2));

                bounceAngle = normalizeRelativeIntersectionY * (maxAngle * Mathf.Deg2Rad);
            }else if (collidedWithComputer)
            {
                collidedWithComputer = false;
                float relativeIntersectY = paddleComputer.transform.localPosition.y - transform.localPosition.y;
                float normalizeRelativeIntersectionY = (relativeIntersectY / (computerPaddleHeight / 2));
                bounceAngle = normalizeRelativeIntersectionY * (maxAngle * Mathf.Deg2Rad);

            }else if (collidedWithWall)
            {
                collidedWithWall = false;
                bounceAngle = -bounceAngle;

            }
        }

    }

    float GetRandomBounceAngle(float minDegrees = 160f, float maxDegrees = 260f)
    {
        float minRad = minDegrees * Mathf.PI / 180;
        float maxRad = maxDegrees * Mathf.PI / 180;

        return Random.Range(minRad, maxRad);
    }

}
