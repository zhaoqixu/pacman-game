using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    public CharacterController player;
    public AIController agent;
    public int numOfItemRemaining = 10;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public EnemyBehaviourScript enemy;

    float enemySpeed = 7.5f;
    float playerSpeed = 15.0f;
    float agentSpeed = 15.0f;
    private Vector3[] alcovePositions = new Vector3[10];
    public bool playerCaptured = false;
    public bool aiCaptured = false;

    public EnemyBehaviourScript topEnemy;
    public EnemyBehaviourScript bottomEnemy;


    // positions of the items
    public List<Vector3> itemPositions = new List<Vector3>();


    // Use this for initialization
    void Start () {
        // set alcoves positions
        setAlcovePosition();
        // random initial position
        Vector3 playerInitPos = randPos();
        Vector3 aiInitPos;
        do
        {
            aiInitPos = randPos();

        } while (aiInitPos.Equals(playerInitPos));
        // create player
        player = CreatePlayer(playerInitPos, playerSpeed);
        // create AI
        agent = CreateAgent(aiInitPos, agentSpeed);
        // populate enemies
        CreateTopEnemy();
        CreateBottomEnemy();
    }

    // Update is called once per frame
    void Update () {
        if (GameOverCondition())
        {
            GameOver();
        }
    }

    private void setAlcovePosition()
    {
        alcovePositions[0] = new Vector3(45, 0, 5);
        alcovePositions[1] = new Vector3(45, 0, -15);
        alcovePositions[2] = new Vector3(45, 0, -35);
        alcovePositions[3] = new Vector3(45, 0, -55);
        alcovePositions[4] = new Vector3(45, 0, -75);
        alcovePositions[5] = new Vector3(-45, 0, 5);
        alcovePositions[6] = new Vector3(-45, 0, -15);
        alcovePositions[7] = new Vector3(-45, 0, -35);
        alcovePositions[8] = new Vector3(-45, 0, -55);
        alcovePositions[9] = new Vector3(-45, 0, -75);
    }

    public Vector3 randPos()
    {
        int r = Random.Range(0, 10);
        return alcovePositions[r];
    }

    public void CreateTopEnemy()
    {
        Vector3 pos = new Vector3();
        float speed = 0;

        if (Random.Range(0, 2) == 0)
        {
            pos = new Vector3(23.5f, 2.3f, 18);
            speed = enemySpeed;
        } else
        {
            pos = new Vector3(23.5f, 2.3f, -88);
            speed = -enemySpeed;
        }

        EnemyBehaviourScript newEnemy = Instantiate(enemy) as EnemyBehaviourScript;
        newEnemy.transform.parent = transform;
        newEnemy.transform.position = pos;
        newEnemy.initSpeed = speed;
        newEnemy.speed = speed;
        newEnemy.initPos = pos;
        topEnemy = newEnemy;
    }

    public void CreateBottomEnemy()
    {
        Vector3 pos = new Vector3();
        float speed = 0;

        if (Random.Range(0, 2) == 0)
        {
            pos = new Vector3(-23.5f, 2.3f, 18);
            speed = enemySpeed;
        }
        else
        {
            pos = new Vector3(-23.5f, 2.3f, -88);
            speed = -enemySpeed;
        }

        EnemyBehaviourScript newEnemy = Instantiate(enemy) as EnemyBehaviourScript;
        newEnemy.transform.parent = transform;
        newEnemy.transform.position = pos;
        newEnemy.initSpeed = speed;
        newEnemy.speed = speed;
        newEnemy.initPos = pos;
        bottomEnemy = newEnemy;
    }


    public CharacterController CreatePlayer(Vector3 pos, float speed)
    {
        CharacterController _player = Instantiate(player) as CharacterController;
        _player.name = "player";
        _player.transform.parent = transform;
        _player.transform.position = pos;
        _player.speed = speed;
        return _player;
    }

    public AIController CreateAgent(Vector3 pos, float speed)
    {
        AIController _agent = Instantiate(agent) as AIController;
        _agent.name = "AIagent";
        _agent.transform.parent = transform;
        _agent.transform.position = pos;
        _agent.initPos = pos;
        _agent.speed = speed;
        return _agent;
    }

    private bool GameOverCondition()
    {
        if (numOfItemRemaining == 0 || (playerCaptured && aiCaptured))
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (player.itemCollected > agent.itemCollected)
        {
            gameOverText.text = "You Win";
        } else if(player.itemCollected < agent.itemCollected)
        {
            gameOverText.text = "AI Win";
        } else
        {
            gameOverText.text = "Draw";
        }
        Time.timeScale = 0;
    }
}
