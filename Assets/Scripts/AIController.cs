using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using HTNplanner;

public class AIController : MonoBehaviour {

    public int itemCollected = 0;
    public int numOfTraps = 2;
    public bool captured = false;
    GameManagerScript GMS;
    public NavMeshAgent agent;
    public Vector3 nextItemPos = new Vector3();
    public Vector3 initPos = new Vector3();

    private Queue<List<string>> actionQueue;
    private List<string> currentAction;

    public float speed;

    List<string> plan;
    public WorldModelManager worldModelManager;
    private bool busy;

    private float startTime;
    private Vector3 startPos;
    private Vector3 endPos;
    private float length;
    private State worldState;


    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(initPos);
        nextItemPos = FindNearestItem();
        actionQueue = new Queue<List<string>>();

        if (worldModelManager)
        {
            worldModelManager.UpdateKnowledge("at", "alcove0", true);
        }

        busy = true;
    }

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update () {

        if (busy)
        {
            ContinueAction();
        }
        else if (actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            DoNextAction(currentAction);
        } else
        {
            try
            {
                SearchandExecutePlan(plan);
            }
            catch (Exception e) { }
        }
    }

    public Vector3 FindNearestItem()
    {
        float minDistance = 1000;
        Vector3 nearestPos = new Vector3();
        for (int i = 0; i < GMS.itemPositions.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, GMS.itemPositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPos = GMS.itemPositions[i];
            }
        }
        return nearestPos;
    }

    private void DoNextAction(List<string> action)
    {
        switch (action[0])
        {
            case "MoveTo":
                startPos = transform.position;
                endPos = new Vector3(startPos.x, 0, startPos.z);
                startTime = Time.time;
                length = Vector3.Distance(startPos, endPos);
                busy = true;
                State state = (worldState != null) ? new State(worldState) : null;
                worldModelManager.UpdateKnowledge("at", state.GetStateOfVar("at")[0], false);
                worldModelManager.UpdateKnowledge("at", action[1], true);
                break;
            case "Hide":
                busy = true;
                break;
            case "Finish":
                busy = true;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            itemCollected++;
            GMS.numOfItemRemaining--;
            for (int i = 0; i < GMS.itemPositions.Count; i++)
            {
                if (GMS.itemPositions[i] == other.transform.position)
                {
                    GMS.itemPositions.RemoveAt(i);
                    break;
                }
            }
            Destroy(other.gameObject);
        }
    }

    private void TrapPlayer()
    {
        Vector3 pos = new Vector3();
        pos = GMS.randPos();
        GMS.player.transform.position = pos;
    }

    private void ContinueAction()
    {
        nextItemPos = FindNearestItem();
        agent.SetDestination(nextItemPos);

        float distTopEnemy = Vector3.Distance(GMS.topEnemy.transform.position, this.transform.position);
        float distBottomEnemy = Vector3.Distance(GMS.bottomEnemy.transform.position, this.transform.position);
        float distPlayer = Vector3.Distance(GMS.player.transform.position, this.transform.position);
        if ((distPlayer < 20 || distTopEnemy < 20 || distBottomEnemy < 20) && numOfTraps > 0 && UnityEngine.Random.Range(0, 20) == 1)
        {
            if (distPlayer < distTopEnemy && distPlayer < distBottomEnemy)
            {
                TrapPlayer();
                numOfTraps--;
            }
            else if (distTopEnemy < distBottomEnemy)
            {
                Destroy(GMS.topEnemy.gameObject);
                GMS.CreateTopEnemy();
                numOfTraps--;
            }
            else
            {
                Destroy(GMS.bottomEnemy.gameObject);
                GMS.CreateBottomEnemy();
                numOfTraps--;
            }
        }
    }

        public void SearchandExecutePlan(List<string> plan)
    {
        actionQueue.Clear();
        foreach (string step in plan)
        {
            List<string> action = new List<string>();
            if (step.Contains(","))
            {
                action.Add(step.Substring(1, step.IndexOf(',') - 1));

                string stepRemainder = step.Substring(step.IndexOf(',') + 2);
                while (stepRemainder.Contains(","))
                {
                    action.Add(stepRemainder.Substring(0, stepRemainder.IndexOf(',')));
                    stepRemainder = step.Substring(step.IndexOf(',') + 2);
                }
                action.Add(stepRemainder.Substring(0, stepRemainder.IndexOf(')')));
            }
            else
            {
                action.Add(step.Substring(1, step.IndexOf(')') - 1));
            }

            actionQueue.Enqueue(action);
        }
    }

    public void SetEnable()
    {
        enabled = false;
    }
}
