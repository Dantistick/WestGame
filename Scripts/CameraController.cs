using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : Entity
{
    [SerializeField] private Transform player;
    private Vector3 pos;
        
    void Update()
    {
        pos = player.position;
        pos.z = -10f;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
    }

    private void Awake()
    {
        if(!player) 
        {
            player = FindObjectOfType<Hero>().transform;
        }
    }
}
