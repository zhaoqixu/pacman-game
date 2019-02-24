using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public int itemCollected = 0;
    private int numOfTraps = 2;
    GameManagerScript GMS;

    public float speed;
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update () {
        Move();
        
        if (Input.GetKeyDown("space") && numOfTraps > 0)
        {
            float distTopEnemy = Vector3.Distance(GMS.topEnemy.transform.position, this.transform.position);
            float distBottomEnemy = Vector3.Distance(GMS.bottomEnemy.transform.position, this.transform.position);
            float distAgent = Vector3.Distance(GMS.agent.transform.position, this.transform.position);

            if (distAgent < distTopEnemy && distAgent < distBottomEnemy)
            {
                TrapAgent();
                numOfTraps--;
            } else if (distTopEnemy < distBottomEnemy)
            {
                Destroy(GMS.topEnemy.gameObject);
                GMS.CreateTopEnemy();
                numOfTraps--;
            } else
            {
                Destroy(GMS.bottomEnemy.gameObject);
                GMS.CreateBottomEnemy();
                numOfTraps--;
            }
        }
    }

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    private void Move()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;
        transform.Translate(straffe, 0, translation);
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

    private void TrapAgent()
    {
        Vector3 pos = new Vector3();
        pos = GMS.randPos();
        GMS.agent.transform.position = pos;
        GMS.agent.agent.Warp(pos);
        GMS.agent.nextItemPos = GMS.agent.FindNearestItem();
    }

    public void SetEnable()
    {
        enabled = false;
    }
}
