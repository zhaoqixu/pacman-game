using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    GameManagerScript GMS;
    private float rotateSpeed = 5f;
    // Use this for initialization
    void Start()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        SelfRotate();
    }

    private void SelfRotate()
    {
        transform.Rotate(Vector3.left * rotateSpeed);
        transform.Rotate(Vector3.forward * rotateSpeed);
    }
}
