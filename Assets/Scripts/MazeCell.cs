using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour {

    public Projectile projectilePrefab;
    GameManagerScript GMS;

    // Use this for initialization
    void Start () {
        CreateProjectile();
    }

    private void Awake()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void CreateProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab) as Projectile;
        newProjectile.transform.parent = transform;
        newProjectile.transform.localPosition = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), -0.2f);
        GMS.itemPositions.Add(newProjectile.transform.position);
    }

}
