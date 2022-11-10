using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FlotonScript : MonoBehaviour
{

    public int level;

    public Rigidbody2D rb;
    public Rigidbody2D hook;
    public GameObject fadeCamera;

    public List<GameObject> planetList;
    public GameObject targetPlanet;
    private List<Vector2> forceList = new List<Vector2>();

    public float releaseTime = .15f;
    public float maxDragDistance = 2f;

    public float cameraWidth = 55;
    public float cameraHeight = 20;

    private bool isPressed;
    private bool released;

    private float xDif;
    private float yDif;
    private float distance;
    private float gravity;
    private float angle;
    private Vector2 gravityForce;


    private void Start()
    {
       isPressed = false;
       released = false;

       cameraWidth = 55;
      cameraHeight = 20;
}

    void Update(){

        if (!this)
        {
            print("nope");
        }

        if (GetComponent<Transform>().position.x<-cameraWidth || GetComponent<Transform>().position.x >  cameraWidth || GetComponent<Transform>().position.y < - cameraHeight || GetComponent<Transform>().position.y > cameraHeight)
        {

            //print(GetComponent<Transform>().position);
            reloadLevel();
        }
        

        if (GetComponent<Transform>().position.x < -cameraWidth)
        {

            
            //reloadLevel();
        }

        if (isPressed){
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

                Vector2 displacement = planet.GetComponent<Transform>().position - GetComponent<Transform>().position;
                displacement.Normalize();

                distance = Vector2.Distance(planet.GetComponent<Transform>().position, GetComponent<Transform>().position);

                gravity = (float)Variables.Object(planet).Get("Gravity") / Mathf.Pow(distance / 10, 2);
                //angle = Mathf.Atan(yDif / Mathf.Abs(xDif));
                gravityForce = gravity * displacement;

                //print(gravityForce);

                //forceList.Add(new Vector2(gravity * Mathf.Cos(angle), gravity * Mathf.Sin(angle)));
                forceList.Add(gravityForce);
            }

            Vector2 totalForce = new Vector2(0, 0);

            foreach(Vector2 force in forceList)
            {
                //print (force);
                totalForce += force;
            }

            GetComponent<ConstantForce2D>().force = totalForce;
       
                //print(gravity + "         " + xDif + "         " + distance + "         " + angle+ "        " + GetComponent<ConstantForce2D>().force);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("test");

        if(collision.gameObject == targetPlanet)
        {
            print ("success");
        }
        else
        {
            foreach (GameObject planet in planetList)
            {
                if (collision.gameObject == planet)
                {
                    reloadLevel();
                }
            }
        }
    }

    public void reloadLevel()
    {
        fadeCamera.GetComponent<FadeCamera>().OnGUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    IEnumerator Release(){
        yield return new WaitForSeconds(releaseTime);
       
        GetComponent<SpringJoint2D>().enabled = false;
       
      
       
        //print("released");
    }
    
}
