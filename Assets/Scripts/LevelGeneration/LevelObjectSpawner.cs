using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectSpawner : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject[] objects;
    // Start is called before the first frame update
    private void Awake()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

    }
    void Start()
    {
     
        int rand = Random.Range(0, objects.Length);
      var newTemp= Instantiate(objects[rand], transform.position,Quaternion.Euler(0,0,Random.Range(0,360)));
        if (newTemp!=null) 
        {

            newTemp.transform.parent = this.transform;
        }
  
      
    }

    // Update is called once per frame
    void Update()
    {
         if(Vector3.Distance(transform.parent.gameObject.transform.position, playerObject.transform.position) > 100){
           // Destroy(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }
       
        //use a physics2d check to see if player is inside area, if the player is inside area, check 6  points around and spawn  levels if they do not exist already, use a physics check to see for other level elements


    }
}
