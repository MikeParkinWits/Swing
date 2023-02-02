using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrebuiltTemplateController : MonoBehaviour
{
    public GameObject playerObject;
  
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {



        if (Vector3.Distance(transform.parent.gameObject.transform.position, playerObject.transform.position) > 100)
        {
           
            Destroy(transform.parent.gameObject);
        }
    }


    

  
}
