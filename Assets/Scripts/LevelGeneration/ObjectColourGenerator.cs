using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColourGenerator : MonoBehaviour
{
    public GameManagerMike gm;
   public Color[] RedPallete= { new Color(255,0,0,255),new Color(215,0,0,255),new Color(198,0,0, 255), new Color(183,0,0, 255),new Color (155,0,0, 255) };
    public Color[] GreenPallete = { new Color(193, 255, 28, 255), new Color(155, 233, 49, 255), new Color(120, 210, 61, 255), new Color(86, 186, 68, 255), new Color(58, 163, 70, 255) };
    public Color[] BluePallete = { new Color(141, 189, 255, 255), new Color(100, 161, 244, 255), new Color(73, 146, 241, 255), new Color(59, 126, 217, 255), new Color(39, 109, 203, 255) };


    public Color[] OrangePallete = { new Color(255, 175, 122, 255), new Color(255, 157, 92, 255), new Color(255, 139, 61, 255), new Color(255, 120, 31, 255), new Color(255, 102, 0, 255) };
    // Start is called before the first frame update
    void Awake()
    {
        gm = FindObjectOfType<GameManagerMike>();
       

        
    }
    private void Start()
    {
        switch (gm.colour)
        {
            case 0:
               // this.gameObject.GetComponent<Renderer>().material.color= RedPallete[Random.Range(0, 4)];
              //  transform.GetComponent<Renderer>().material.color = RedPallete[Random.Range(0, 4)];
              gameObject.GetComponent<Renderer>().material.SetColor("_Color", RedPallete[Random.Range(0, 5)]);
   
                break;
            case 1:
                //this.gameObject.GetComponent<Renderer>().material.color = GreenPallete[Random.Range(0, 4)];
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", GreenPallete[Random.Range(0, 5)]);
                //   transform.GetComponent<Renderer>().material.color = GreenPallete[Random.Range(0, 4)];
                break;
            case 2:
               // this.gameObject.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", BluePallete[Random.Range(0, 5)]);
                //  transform.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];
                break;

            case 3:
               // this.gameObject.GetComponent<Renderer>().material.color = OrangePallete[Random.Range(0, 4)];
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", OrangePallete[Random.Range(0, 5)]);
                //  transform.GetComponent<Renderer>().material.color =OrangePallete[Random.Range(0, 4)];
                break;

            default:
             //   this.gameObject.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];
                //  transform.GetComponent<Renderer>().material.color = BluePallete[Random.Range(0, 4)];
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", BluePallete[Random.Range(0, 5)]);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
