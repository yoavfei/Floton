using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlotonScript : MonoBehaviour
{

    public Rigidbody2D rb;
    public Rigidbody2D hook;

    public List<GameObject> planetList;
    private List<Vector2> forceList = new List<Vector2>();

    public float releaseTime = .15f;
    public float maxDragDistance = 2f;

    private bool isPressed = false;
    private bool released = false;

    private float xDif;
    private float yDif;
    private float distance;
    private float gravity;
    private float angle;


    private void Start()
    {
       
    }

    void Update(){



        if(isPressed){
            //print("running");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(Vector3.Distance(mousePos, hook.position) > maxDragDistance){
                rb.position = hook.position + (mousePos - hook.position ).normalized *maxDragDistance;
            }
            else{
                rb.position = mousePos;
            }
            
        }
    
        if(released)
        {
            forceList.Clear();

            foreach(GameObject planet in planetList)
            {
                xDif = planet.GetComponent<Transform>().position.x - GetComponent<Transform>().position.x;

                yDif = planet.GetComponent<Transform>().position.y - GetComponent<Transform>().position.y;

                distance = Vector2.Distance(planet.GetComponent<Transform>().position, GetComponent<Transform>().position);

                gravity = (float)Variables.Object(planet).Get("Gravity") / Mathf.Pow(distance / 10, 2);
                angle = Mathf.Acos(xDif / distance);

                forceList.Add(new Vector2(gravity * Mathf.Cos(angle), gravity * Mathf.Sin(angle)));
            }

            Vector2 totalForce = new Vector2(0, 0);

            foreach(Vector2 force in forceList)
            {
                totalForce += force;
            }

            GetComponent<ConstantForce2D>().force = totalForce;
       
                print(gravity + "         " + xDif + "         " + distance + "         " + angle+ "        " + GetComponent<ConstantForce2D>().force);
            //  GetComponent<ConstantForce2D>().force = new Vector2( xDif* planet.GetComponent <Variables>().Gravity/ Mathf.Pow(distance,2), yDif / Mathf.Pow(distance, 2));
            //  GetComponent<ConstantForce2D>().force = new Vector2(xDif * gravity / Mathf.Pow(distance, 2), yDif * gravity / Mathf.Pow(distance, 2));


            
        }
      
       

    }

    void OnMouseDown(){
        isPressed = true;
        rb.isKinematic = true;


    }

    void OnMouseUp(){
        isPressed = false;
        rb.isKinematic = false;
       
        StartCoroutine(Release());
        released = true;
    }

    IEnumerator Release(){
        yield return new WaitForSeconds(releaseTime);
       
        GetComponent<SpringJoint2D>().enabled = false;
       
      
       
        //print("released");
    }
    
}
