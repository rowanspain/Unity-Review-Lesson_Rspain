using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemCollect : NetworkBehaviour
{
    private Dictionary<Item.VegetableType, int> ItemInventory - new Dictionary<Item.VegetableType, int>();


    public delegate void CollectItem(Item.VegetableType item);
    public static event CollectItem ItemCollected;

    Collider itemCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        rbPlayer = GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        foreach(Item.VegetableType item in System.Enum.GetValues(typeof(Item.VegetableType)))
        {
            ItemInventory.Add(item, 0);
        }
        
    }

    void Update()
    {
        if (itemCollider && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space bar and item collected");
            Item item = other.gameObject.GetComponent<Item>();
            AddToInventory(item);
            PrintInventory();

            CmdItemCollected(item.typeOfVeggie);

        }
    }

    [Command]
    void CmdItemCollected(Item.VegetableType itemType)
    {
        Debug.Log("CommandItemCollected: " + itemType);
        RpcItemCollected(itemType);
    }

    [ClientRpc]
    void RpcItemCollected(Item.VegetableType itemType)
    {
        Debug.Log("CommandItemCollected: " + itemType);
        ItemCollected?.Invoke(itemType);
    }

    // Update is called once per frame
   private void OnTriggerEnter(Collider other)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(other.CompareTage("Item"))
        {
            itemCollider = other;
        }
    }

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        float horMove = Input.GetAxis("Horizontal");
        float verMove = Input.GetAxis("Vertical");

        direction = new Vector3(horMove, 0, verMove);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!LocalPlayer)
        {
            return;
        }
       
        if (other.CompareTag("Item") && Input.GetKeyDown(KeyCode.Space))
        {
            itemCollider = other;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(other.CompareTag("Item"))
        {
            itemCollider = null;
        }
    }

    private void AddToInventory(Item item)
    {
        ItemInventory[item.typeOfVeggie]++;
    }

    private void PrintInventory()
    {
        string output = "";

        foreach (KeyValuePair<Item.VegetableType, int> kvp in ItemInventory)
        {
            output += string.Format("{0}: {1} ", kvp.Key, kvp.Value);
        }
        Debug.Log(output);
    }
}
