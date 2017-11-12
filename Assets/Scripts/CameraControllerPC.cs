﻿using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class CameraControllerPC : MonoBehaviour
{
    //Rigidbody rb;

    public GameObject prefab1;

    public Vector3 slpoint1;
    public Vector3 slpoint2;

    //Placing Prefabs
    public Transform placeObj, createObj, //Object that appears at mouse position when building
        place_obj_turret, place_obj_wall, place_obj_hut; //Prefabs for what to create

    Vector3 startBuildPoint;

    public enum Building { None, Wall, Turret, Tepee };

    public Building building_placing = Building.None;
    float placeRotation = 0;
    public bool inInterface = false;

    float viewScale = 1;

    Camera cam;

    Vector3 mouse_start_pos;
    Vector3 view_start_pos;

    private Vector2 orgBoxPos = Vector2.zero;
    private Vector2 endBoxPos = Vector2.zero;

    public Transform target;

    Rigidbody rb;

    Transform childCam;

    // Use this for initialization
    void Start()
    {
        //rb = transform.GetComponent<Rigidbody>();
        childCam = transform.GetChild(0); //.gameObject;
        cam = childCam.GetComponent<Camera>();

        rb = transform.GetComponent<Rigidbody>();

        //createObj = transform.GetChild(1);
        //PlayerSettings.virtualRealitySupported = false;
    }
    /*
    void OnGUI()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        GUILayout.EndArea();
    }
    
    
    */

    public void setInInterfaceTrue()
    {
        inInterface = true;
    }
    public void setInInterfaceFalse()
    {
        inInterface = false;
    }

    // Update is called once per frame
    void Update()
    {

        float rx = transform.eulerAngles.x;
        float ry = transform.eulerAngles.y;
        float rz = transform.eulerAngles.z;
        childCam.transform.position = transform.position + Quaternion.AngleAxis(ry, Vector3.up) * (childCam.transform.localPosition + new Vector3(0, viewScale, -viewScale))/2;
            //SetPositionAndRotation(transform.position + new Vector3(0,viewScale,viewScale) , new Vector3(45,0,0));

        //Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y ));

        Transform wallTransform, turretTransform, tepeeTransform;
        wallTransform = createObj.GetChild(0);
        turretTransform = createObj.GetChild(1);
        tepeeTransform = createObj.GetChild(2);
        
        wallTransform.gameObject.SetActive(false);
        turretTransform.gameObject.SetActive(false);
        tepeeTransform.gameObject.SetActive(false);
        ///Credit for Following Switch Statement Code Goes to Andrew Ajemian////////////////////////////////////
        switch (building_placing)
        {
            case Building.Tepee:
                placeObj = place_obj_hut;
                tepeeTransform.gameObject.SetActive(true); //except this, Arthur Baney wrote this
                break;
            case Building.Wall:
                placeObj = place_obj_wall;
                wallTransform.gameObject.SetActive(true); //except this, Arthur Baney wrote this
                break;
            case Building.Turret:
                placeObj = place_obj_turret;
                turretTransform.gameObject.SetActive(true); //except this, Arthur Baney wrote this
                break;
            case Building.None:
                placeObj = null;
                break;
            default:
                placeObj = null;
                break;
        }
        //////////////////////////End Andrew's Code///////////////////////////////////////

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{

        /*
        var t = reference;
        if (t == null)
            return;
        // Get the current Y position of the reference space
        float refY = t.position.y;

        // Create a plane at the Y position of the Play Area
        // Then create a Ray from the origin of the controller in the direction that the controller is pointing
        Plane plane = new Plane(Vector3.up, -refY);
        */

        //If the mouse is not hovering over a button
        if (!inInterface)
        {
            //set the active state of the hover object to true
            createObj.gameObject.SetActive(true);
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                createObj.position = hit.point;
                createObj.eulerAngles = new Vector3(0, placeRotation, 0);
                //CLICK TO PLACE BUILDINGS, CREATE BUILDINGS
                if (building_placing != Building.None)
                {
                    //Stop placing buildings when right clicked
                    if (Input.GetMouseButtonDown(1))
                    {
                        building_placing = Building.None;
                    }

                    //Place buildings when left clicked
                    if (Input.GetMouseButtonDown(0))
                    {
                        startBuildPoint = hit.point;
                        //Transform newobj = Instantiate(placeObj, createObj.position, new Quaternion(0, 0, 0, 0));
                        //newobj.eulerAngles = new Vector3(0, placeRotation, 0);
                    }
                    //When left click released if mouse point far from start point then create long wall
                    if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    {
                        float dis = Vector3.Distance(startBuildPoint, hit.point);
                        if (dis > .5f && placeObj == place_obj_wall)
                        {
                            float xdis = hit.point.x - startBuildPoint.x;
                            float zdis = hit.point.z - startBuildPoint.z;
                            int increments = (int)(dis / .4f);

                            float xpos = startBuildPoint.x;
                            float zpos = startBuildPoint.z;

                            float dir = -Mathf.Atan2(hit.point.z - startBuildPoint.z, hit.point.x - startBuildPoint.x) * 180 / Mathf.PI;
                            //Angle(new Vector2(startBuildPoint.x, startBuildPoint.z) , new Vector2(hit.point.x, hit.point.z));
                            
                            for (int i = 0; i <= Math.Min(increments, 10); i++)
                            {
                                Transform newobj = Instantiate(placeObj, new Vector3(xpos, 0, zpos), new Quaternion(0, 0, 0, 0)); // startBuildPoint.y
                                newobj.tag = "PC Player's Building";
                                newobj.eulerAngles = new Vector3(0, dir, 0);
                                if (!Input.GetMouseButtonUp(0))
                                {
                                    GameObject.Destroy(newobj.gameObject, .02f);
                                }
                                xpos += Mathf.Cos(-Mathf.Deg2Rad * dir) * (dis / increments); // xdis / increments;
                                zpos += Mathf.Sin(-Mathf.Deg2Rad * dir) * (dis / increments);  //zdis / increments;
                            }
                            
                        }
                        else
                        {
                            Transform newobj = Instantiate(placeObj, new Vector3(startBuildPoint.x, 0, startBuildPoint.z), new Quaternion(0, 0, 0, 0));
                            newobj.tag = "PC Player's Building";
                            newobj.eulerAngles = new Vector3(0, placeRotation, 0);
                            if (!Input.GetMouseButtonUp(0))
                            {
                                GameObject.Destroy(newobj.gameObject, .02f);
                            }
                        }
                    }
                }

                if (building_placing == Building.None)
                {
                    //RIGHT CLICK TO TELL UNITS WHERE TO MOVE
                    if (Input.GetMouseButtonDown(1))
                    {
                        //GET CLICK POINT
                        Vector3 targetPoint = hit.point;

                        //GET PC PLAYER'S UNITS LIST
                        GameObject[] list = GameObject.FindGameObjectsWithTag("PC Player's Unit");

                        //CREATE LIST FOR SELECTED UNITS
                        ArrayList listSelected = new ArrayList();

                        //GET ALL UNITS THAT ARE SELECTED AND STORE IN LIST
                        for (int i = 0; i < list.Length; i++)
                        {
                            if (list[i].GetComponent<UnitController>().selected)
                            {
                                listSelected.Add(list[i]);
                            }
                        }

                        float width = Mathf.Sqrt(listSelected.Count); // listSelected.Count; // Mathf.Sqrt(listSelected.Count);

                        //IF THERE'S ONLY ONE UNIT, SEND IT TO TARGET
                        if (listSelected.Count == 1)
                        {
                            ((GameObject)listSelected[0]).BroadcastMessage("Target", targetPoint);
                        }
                        else //IF THERE'S MULTIPLE UNITS, PUT THEM IN FORMATION
                        {
                            int row = 0, col = 0;
                            for (int i = 0; i < listSelected.Count; i++)
                            {
                                ((GameObject)listSelected[i]).BroadcastMessage("Target", targetPoint + new Vector3((col) * .3f - width / 2 * .3f, 0, (row) * .3f - width / 2 * .3f));
                                slpoint1 = new Vector3(0, 0, 0);
                                slpoint2 = new Vector3(0, 0, 0);
                                col++;
                                if (col > width)
                                {
                                    col = 0;
                                    row++;
                                }
                            }

							/*if (!FindObjectOfType<AudioManager> ().isPlaying ("SoldiersMarching")) 
							{
								FindObjectOfType<AudioManager> ().Play ("SoldiersMarching");
							}*/
                        }
                    }

                    //LEFT CLICK TO PLACE STARTING POINT OF RECTANGLE
                    if (Input.GetMouseButtonDown(0))
                    {
                        slpoint1 = hit.point;
                        //GameObject x = Instantiate(prefab1, slpoint1, Quaternion.identity);
                        //Debug.Log(slpoint2);
                    }

                    //WHILE LEFT MOUSE BUTTON IS HELD TO PLACE ENDING POINT OF RECTANGLE
                    if (Input.GetMouseButton(0))
                    {
                        //Select units inside selection rectangle
                        GameObject[] list = GameObject.FindGameObjectsWithTag("PC Player's Unit");
                        for (int i = 0; i < list.Length; i++)
                        {
                            Rect rect = new Rect(slpoint1.x, slpoint1.z, slpoint2.x - slpoint1.x, slpoint2.z - slpoint1.z);
                            if (rect.Contains(new Vector2(list[i].GetComponent<Transform>().position.x, list[i].GetComponent<Transform>().position.z), true))
                            {
                                list[i].BroadcastMessage("Select");
                            }
                            else
                            {
                                list[i].BroadcastMessage("DeSelect");
                            }
                        }
                        slpoint2 = hit.point;
                    }
                }
            }
            //}
            /*
            if (Input.GetMouseButtonDown(0))
            {
                GameObject x = Instantiate(prefab1, new Vector3(mouseWorldPos.x, 5, mouseWorldPos.z), Quaternion.identity);
                x.transform.localScale = new Vector3(10, 10, 10);
                Debug.Log(new Vector3(0, 10, 0));

                slpoint1 = mouseWorldPos + new Vector3(0, 10, 0);
            }

            if (Input.GetMouseButton(0))
            {
                //GameObject x = Instantiate(prefab, new Vector2(Input.mousePosition.x, Input.mousePosition.y, Quaternion.identity);
                slpoint2 = mouseWorldPos + new Vector3(0, 10, 0);
            }*/
        }
        else
        {
            //If the mouse is hovering over a button then set the active state of the hover object to false
            createObj.gameObject.SetActive(false);
        }
        //WHEN MIDDLE MOUSE BUTTON IS PRESSED MOVE VIEW
        if (Input.GetMouseButtonDown(2))
        {
            mouse_start_pos = Input.mousePosition;
            view_start_pos = transform.position;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 vec = Quaternion.AngleAxis(ry, Vector3.up) * (new Vector3(
                 -(Input.mousePosition.x - mouse_start_pos.x) / 500 * (transform.position.y + 10), 0,
                   -(Input.mousePosition.y - mouse_start_pos.y) / 500 * (transform.position.y + 10)
                   ) * viewScale )
                + view_start_pos;
                //new Vector3(view_start_pos.x,view_start_pos.y,view_start_pos.z)
                ;
            //new Vector3(view_start_pos.x + (-Input.mousePosition.x - mouse_start_pos.x) / 1000, transform.position.y , view_start_pos.z + (-Input.mousePosition.y - mouse_start_pos.y) / 1000);
            transform.position = vec;
        }
        float scale = transform.position.y / 2;


        float speed = 500;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            placeRotation -= Input.GetAxisRaw("Mouse ScrollWheel") * 100;
            Vector3 moveVector = Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(Input.GetAxis("Horizontal") * speed * (viewScale + 10) * Time.deltaTime,
                0, Input.GetAxis("Vertical") * speed * (viewScale + 10) * Time.deltaTime);
            rb.AddForce(moveVector);
            //rb.AddForce(Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(Input.GetAxis("Horizontal") * 2000 * scale,
            //                  0, Input.GetAxis("Vertical") * 2000 * scale ) * Time.deltaTime);
        }
        else
        {
            viewScale -= Input.GetAxis("Mouse ScrollWheel") * viewScale * 120 * Time.deltaTime;
            //MOVE VIEW USING ARROW KEYS OR WASD

            Vector3 moveVector = Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(Input.GetAxis("Horizontal") * speed * (viewScale + 10) * Time.deltaTime, 
                0, Input.GetAxis("Vertical") * speed * (viewScale + 10) * Time.deltaTime);
            rb.AddForce(moveVector); /*Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(Input.GetAxis("Horizontal") * 2000 * scale*/
                                  ///*-Input.GetAxisRaw("Mouse ScrollWheel") * 50000 * scale*/ 0,
            //Input.GetAxis("Vertical") * 2000 * scale + Input.GetAxisRaw("Mouse ScrollWheel") * 50000 * scale) * Time.deltaTime);
        }

        if (viewScale < 1)
            viewScale = 1;
        if (viewScale > 40)
            viewScale = 40;
        float minheight = 0;

        //SET MINIMUM HEIGHT FOR VIEW
        if (transform.position.y < minheight)
        {
            transform.position = Quaternion.AngleAxis(ry, Vector3.up) * new Vector3(transform.position.x, minheight, transform.position.z);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        /*
        transform.position += Quaternion.AngleAxis(ry, Vector3.up) 
            * new Vector3( Input.GetAxis("Horizontal") * 5 * scale, 
                          -Input.GetAxisRaw("Mouse ScrollWheel") * 100 * scale, 
                           Input.GetAxis("Vertical") * 5 * scale + Input.GetAxisRaw("Mouse ScrollWheel") * 100 * scale) * Time.deltaTime;
        */
        //Rotate the view with Q and E or < and >
        ry -= Input.GetAxisRaw("Horizontal Rotation") * 200 * Time.deltaTime;

        transform.eulerAngles = new Vector3(rx, ry, rz);

        //DRAW SELECTION RECTANGLE
        //x1 to x2 on z1
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint1.z), Color.blue);

        //x1 to x2 on z2
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint2.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint2.z), Color.blue);

        //z1 to z2 on x1
        DrawLine(new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint1.x, slpoint1.y + .1f, slpoint2.z), Color.blue);

        //z1 to z2 on x2
        DrawLine(new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint1.z),
                 new Vector3(slpoint2.x, slpoint1.y + .1f, slpoint2.z), Color.blue);

        //inInterface = false;
    }

    void DrawRectangle(Vector3 point1, Vector3 point2)
    {
        //x1 to x2 on z1
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point1.z), Color.red);

        //x1 to x2 on z2
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point1.z), Color.blue);

        //z1 to z2 on x1
        DrawLine(new Vector3(point1.x, point1.y + .1f, point1.z),
                 new Vector3(point1.x, point1.y + .1f, point2.z), Color.blue);

        //z1 to z2 on x2
        DrawLine(new Vector3(point2.x, point1.y + .1f, point1.z),
                 new Vector3(point2.x, point1.y + .1f, point2.z), Color.blue);
    }

    //CREDIT FOR DRAWLINE SCRIPT GOES TO paranoidray from his answer on http://answers.unity3d.com/questions/8338/how-to-draw-a-line-using-script.html
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.01f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.01f, 0.01f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    //Credit for this Angle script goes to jakobschou from his answer on http://answers.unity3d.com/questions/162177/vector2angles-direction.html
    float Angle(Vector2 a, Vector2 b)
    {
        var an = a.normalized;
        var bn = b.normalized;
        var x = an.x * bn.x + an.y * bn.y;
        var y = an.y * bn.x - an.x * bn.y;
        return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
}
