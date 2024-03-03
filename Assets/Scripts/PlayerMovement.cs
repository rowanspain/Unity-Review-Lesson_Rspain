using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    public float speed = 10.0f;
    public GameObject[] spawnPoints = null;


    // Start is called before the first frame update
    void Start()
    {
        if(!LocalPlayer)
        {
            return;
        }

        rbPlayer = GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    }



    private void Update()
    {
        if (!LocalPlayer)
        {
            return;
        }

        float horMove = Input.GetAxis("Horizontal");
        float verMove = Input.GetAxis("Vertical");

        direction = new Vector3(horMove, 0, verMove);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LocalPlayer)
        {
            return;
        }

        rbPlayer.AddForce(direction * speed, ForceMode.Force);

        if(transform.position.z > 40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 40);
        }
        else if(transform.position.z < -40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -40);
        }
    }

    private void Respawn()
    {
        int index = 0;
        while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f, 1.5f, 1.5f)))
        {
            index++;
        }
        rbPlayer.MovePosition(spawnPoints[index].transform.position);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!LocalPlayer)
        {
            return;
        }

        if (other.CompareTag("Item") && Input.GetKeyDown(KeyCode.Space))
        {
            Item item = other.gameObject.GetComponent<Item>();
            AddToInventory(item);
            PrintInventory();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!LocalPlayer)
        {
            return;
        }

        if (other.CompareTag("Hazard"))
        {
            Respawn();
        }
    }


}
