using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourScript : MonoBehaviour {

    public float initSpeed;
    public float speed;
    public Vector3 initPos;
    GameManagerScript GMS;
    GameObject playerObj;
    GameObject agentObj;
    private Vector3 fov_left_upper = new Vector3();
    private Vector3 fov_left_lower = new Vector3();
    private Vector3 fov_right_upper = new Vector3();
    private Vector3 fov_right_lower = new Vector3();
    NavMeshObstacle obstacle;

    // Use this for initialization
    void Start () {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        playerObj = GameObject.Find("player");
        agentObj = GameObject.Find("AIagent");
        obstacle = GetComponent<NavMeshObstacle>();
    }

    // Update is called once per frame
    void Update () {

        Move();

        obstacle.center = new Vector3(0, 0, Math.Sign(speed)*1.75f);

        if (!checkInObstacle())
        {
            UpdateFOVVolume();
        }

        // try to capture player
        if (playerObj != null && FindAgent(playerObj))
        {
            Debug.Log("Player is captured!");
            GMS.playerCaptured = true;
            Destroy(playerObj);
        }

        // try to capture agent
        if (agentObj != null && FindAgent(agentObj))
        {
            Debug.Log("AI agent is captured!");
            GMS.aiCaptured = true;
            Destroy(agentObj);
        }
    }

    private void Move()
    {
        float translation = speed * Time.deltaTime;
        transform.Translate(0, 0, translation);
    }

    private void UpdateFOVVolume()
    {
        int sign = -Math.Sign(speed);
        fov_left_upper = transform.position + new Vector3(15.5f, 0, sign*11f);
        fov_left_lower = transform.position + new Vector3((-15.5f), 0, sign*11f);
        fov_right_upper = transform.position + new Vector3(15.5f, 0, sign * (-2.5f));
        fov_right_lower = transform.position + new Vector3((-15.5f), 0, sign * (-2.5f));
        Debug.DrawLine(fov_left_upper, fov_left_lower, Color.cyan);
        Debug.DrawLine(fov_right_upper, fov_right_lower, Color.cyan);
        Debug.DrawLine(fov_left_lower, fov_right_lower, Color.cyan);
        Debug.DrawLine(fov_left_upper, fov_right_upper, Color.cyan);
    }

    private bool FindAgent(GameObject obj)
    {
        Vector3 playerPos = obj.transform.position;
        if (speed < 0)
        {
            if (playerPos.x <= fov_left_upper.x
                && playerPos.x >= fov_left_lower.x
                && playerPos.z <= fov_left_lower.z
                && playerPos.z >= fov_right_lower.z)
            {
                return true;
            }
        }

        if (speed > 0)
        {
            if (playerPos.x <= fov_left_upper.x
                && playerPos.x >= fov_left_lower.x
                && playerPos.z >= fov_left_lower.z
                && playerPos.z <= fov_right_lower.z)
            {
                return true;
            }
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        // encounter doorways
        if (other.gameObject.tag == "trigger")
        {
            if(this.transform.position.x > 0)
            {
                GMS.CreateTopEnemy();
            } else {
                GMS.CreateBottomEnemy();
            }
            Destroy(gameObject);
        }

        // encounter small obstacles
        if (other.gameObject.tag == "obstacle")
        {
            int r = UnityEngine.Random.Range(0, 3);
            switch (r)
            {
                // go through the obstacle
                case 0:
                    // no FOV while inside obstacle
                    break;
                // disppear and re-spawn
                case 1:
                    if (this.transform.position.x > 0)
                    {
                        GMS.CreateTopEnemy();
                    }
                    else
                    {
                        GMS.CreateBottomEnemy();
                    }
                    Destroy(gameObject);
                    break;
                // reverse direction
                case 2:
                    speed = -speed;
                    break;
                default:
                    break;
            }
        }
    }

    private bool checkInObstacle()
    {
        Vector3 pos = transform.position;
        if (pos.z >= -38.5 && pos.z <= -31.5)
        {
            return true;
        }
        return false;
    }
}
