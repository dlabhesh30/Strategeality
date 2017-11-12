﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public bool fighting = false;

    // Use this for initialization
    void Start () {
		
	}

    //FaceTowardsObject
    //This script will make the unit face towards another GameObject identified by the "obj" argument
    //Example: can be used for making a warrior unit face its enemy while fighting
    void FaceTowardsObject(GameObject obj)
    {
        //Determine direction to the object
        float dirToObj = -Mathf.Atan2(obj.transform.position.z - transform.position.z, obj.transform.position.x - transform.position.x) * Mathf.Rad2Deg + 90; //Vector2.Angle(new Vector2(transform.position.x, transform.position.z), new Vector2(nearestEnemy.transform.position.x, nearestEnemy.transform.position.z));// * Mathf.Rad2Deg;

        //Make the unit face towards the object
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                                            dirToObj,
                                            transform.eulerAngles.z);
    }

	// Update is called once per frame
	void Update () {
        
        GameObject nearestEnemy = GetNearestEnemyUnit();

        //Fight with nearest enemy unit if it exists
        if (nearestEnemy != null && Vector3.Distance(nearestEnemy.transform.position, transform.position) < 1)
        {
			if (!FindObjectOfType<AudioManager> ().isPlaying ("SwordClash")) 
			{
				FindObjectOfType<AudioManager> ().Play ("SwordClash");
			}
			nearestEnemy.GetComponent<HealthBar>().AddHealth(-1f * Time.deltaTime); //health -=1 * Time.deltaTime;
            fighting = true;
            //Face towards the enemy
            FaceTowardsObject(nearestEnemy);
        }
        else
        {
            //If has no nearest enemy unit in range, check for enemy buildings that are in range
            nearestEnemy = GetNearestEnemyBuilding();

            //Attack closest enemy building if it exists
            if (nearestEnemy != null && Vector3.Distance(nearestEnemy.transform.position, transform.position) < 1)
            {
                nearestEnemy.GetComponent<HealthBar>().AddHealth(-1 * Time.deltaTime);
                fighting = true;
				FindObjectOfType<AudioManager>().Play("SwordClash");
                FaceTowardsObject(nearestEnemy);
            }
            else //If closest enemy building doesn't exist, then do nothing
            {
                fighting = false;
            }
            
        }
    }

    //Get Nearest Enemy Building
    //Returns the nearest enemy building to this unit
    public GameObject GetNearestEnemyBuilding()
    {
        GameObject[] VRBuildings = new GameObject[0];
        GameObject[] PCBuildings = new GameObject[0];
        GameObject[] AIBuildings = new GameObject[0];

        List<GameObject> enemiesBuildings = new List<GameObject>();

        PCBuildings = GameObject.FindGameObjectsWithTag("PC Player's Building");
        VRBuildings = GameObject.FindGameObjectsWithTag("VR Player's Building");
        AIBuildings = GameObject.FindGameObjectsWithTag("AI Player's Building");

        //If this unit is not on the PC Player's team, add all the PC Player's Buildings to the enemiesBuildings arraylist 
        //  (later we can have it check if the two teams are at war first, and have an option to declare war and make peace with different teams)
        if (tag != "PC Player's Unit")
            for (int i = 0; i < PCBuildings.Length; i++)
            {
                enemiesBuildings.Add(PCBuildings[i]);
            }

        //If this unit is not on the VR Player's team, add all the PC Player's Buildings to the enemiesBuildings arraylist
        if (tag != "VR Player's Unit")
            for (int i = 0; i < VRBuildings.Length; i++)
            {
                enemiesBuildings.Add(VRBuildings[i]);
            }

        //If this unit is not on the AI Player's team, add all the PC Player's Buildings to the enemiesBuildings arraylist
        if (tag != "AI Player's Unit")
            for (int i = 0; i < AIBuildings.Length; i++)
            {
                enemiesBuildings.Add(AIBuildings[i]);
            }

        //Find closest enemy building
        GameObject nearestEnemyBuilding = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < enemiesBuildings.Count; i++)
        {
            float dist = Vector3.Distance(enemiesBuildings[i].transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestEnemyBuilding = enemiesBuildings[i];
            }
        }

        return nearestEnemyBuilding;
    }

    //Get Nearest Enemy Unit
    //Returns the nearest enemy unit to this unit
    public GameObject GetNearestEnemyUnit()
    {

        GameObject[] VRUnits = new GameObject[0];
        GameObject[] PCUnits = new GameObject[0];
        GameObject[] AIUnits = new GameObject[0];

        List<GameObject> enemyUnits = new List<GameObject>();

        //Initialize arraylists for the units of the different teams
        PCUnits = GameObject.FindGameObjectsWithTag("PC Player's Unit");
        AIUnits = GameObject.FindGameObjectsWithTag("AI Player's Unit");
        VRUnits = GameObject.FindGameObjectsWithTag("VR Player's Unit");

        //If this unit is not on the PC Player's team, add all the PC Player's Units to the enemyUnits arraylist 
        if (tag != "PC Player's Unit")
            for (int i = 0; i < PCUnits.Length; i++)
            {
                enemyUnits.Add(PCUnits[i]);
            }

        //If this unit is not on the VR Player's team, add all the PC Player's Units to the enemyUnits arraylist 
        if (tag != "VR Player's Unit")
            for (int i = 0; i < VRUnits.Length; i++)
            {
                enemyUnits.Add(VRUnits[i]);
            }

        //If this unit is not on the AI Player's team, add all the PC Player's Units to the enemyUnits arraylist 
        if (tag != "AI Player's Unit")
            for (int i = 0; i < AIUnits.Length; i++)
            {
                enemyUnits.Add(AIUnits[i]);
            }

        //Find nearest enemy unit
        GameObject nearestEnemyUnit = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            float dist = Vector3.Distance(enemyUnits[i].transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestEnemyUnit = enemyUnits[i];
            }
        }

        return nearestEnemyUnit;
    }
}
