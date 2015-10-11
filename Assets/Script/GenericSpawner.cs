using UnityEngine;
using System.Collections.Generic;

public class GenericSpawner : MonoBehaviour {

    public enum SpawnFunct { RANDOM, SIN_X, ANIM_CURVE }
    //Spawning Function
    public SpawnFunct selectedFunction = SpawnFunct.RANDOM;

    // Used to randomly generate X offset
    public float minPosX = 0f;
    public float maxPosX = 0f;
    // Used to randomly generate Y offset
    public float minPosY = 0f;
    public float maxPosY = 0f;
    // Used to set a random spawn frequency
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 1f;
    // The pool of objects we want to spawn
    public GameObject[] objectsPool;
    // Number of elements to spawn: negative means infinite
    public int elemToSpawn = -1;
    public AnimationCurve myCurveX;
    public AnimationCurve myCurveY;

    int elemCounter;
    Queue<GameObject> objectsQueue;

	// Use this for initialization
	void Start () {
        // Populate queue
        objectsQueue = new Queue<GameObject>();
        foreach(GameObject go in objectsPool)
        {
            objectsQueue.Enqueue(go);
        }
        Invoke("Spawn", Random.Range(minSpawnTime, maxSpawnTime));	
	}
	
	// This is called recursevly to spawn an object
	void Spawn () {
	    if((elemToSpawn < 0 || elemCounter++ <= elemToSpawn) && objectsQueue.Count > 0)
        {
            GameObject objectToSpawn = objectsQueue.Dequeue();
            switch (selectedFunction)
            {
                case SpawnFunct.RANDOM:
                    objectToSpawn.transform.position = GetRandomSpawnPos();
                    break;
                case SpawnFunct.SIN_X:
                    objectToSpawn.transform.position = GetSinSpawnPos();
                    break;
                case SpawnFunct.ANIM_CURVE:
                    objectToSpawn.transform.position = GetAnimCurveSpawnPos();
                    break;
            }
            objectToSpawn.SetActive(true);
        }
        Invoke("Spawn", Random.Range(minSpawnTime, maxSpawnTime));
    }

    Vector3 GetRandomSpawnPos()
    {
        Vector3 newPos = transform.position;
        newPos.x += Random.Range(minPosX, maxPosX);
        newPos.y += Random.Range(minPosY, maxPosY);
        return newPos;
    }

    Vector3 GetSinSpawnPos()
    {
        Vector3 newPos = transform.position;
        newPos.x += (maxPosX + minPosX) / 2 + ((maxPosX - minPosX) / 2) * Mathf.Cos(Time.time);
        newPos.y += (maxPosY + minPosY) / 2 + ((maxPosY - minPosY) / 2) * Mathf.Sin(Time.time);
        return newPos;
    }

    Vector3 GetAnimCurveSpawnPos()
    {
        Vector3 newPos = transform.position;
        float curveMinX = myCurveX.keys[0].value, curveMaxX = myCurveX.keys[0].value;
        float curveMinY = myCurveY.keys[0].value, curveMaxY = myCurveY.keys[0].value;
        /* I am assuming here, that all maximum-minimum points in the curve are on a keyframe, since you modify the curve by dragging them */
        foreach (Keyframe k in myCurveX.keys)
        {
            curveMinX = k.value <= curveMinX ? k.value : curveMinX;
            curveMaxX = k.value >= curveMaxX ? k.value : curveMaxX;
        }
        /* I am assuming here, that all maximum-minimum points in the curve are on a keyframe, since you modify the curve by dragging them */
        foreach (Keyframe k in myCurveY.keys)
        {
            curveMinY = k.value <= curveMinY ? k.value : curveMinY;
            curveMaxY = k.value >= curveMaxY ? k.value : curveMaxY;
        }
        // Evaluating spawning_curve at this frame
        float cur_val_x = myCurveX.Evaluate(Time.time);
        // Rescaling value to fit the left/right range
        float normalized_x = (cur_val_x - curveMinX) / (curveMaxX - curveMinX);
        normalized_x = float.IsNaN(normalized_x) ? 0 : normalized_x;
        float x = (minPosX - maxPosX) * normalized_x + maxPosX;
        // Evaluating spawning_curve at this frame
        float cur_val_y = myCurveY.Evaluate(Time.time);
        // Rescaling value to fit the top/bottom range
        float normalized_y = (cur_val_y - curveMinY) / (curveMaxY - curveMinY);
        normalized_y = float.IsNaN(normalized_y) ? 0 : normalized_y;
        float y = (maxPosY - minPosY) * normalized_y + minPosY;
        newPos.x += x;
        newPos.y += y;
        return newPos;
    }

    // Requeue object
    public void RequeueObject(GameObject obj)
    {
        obj.SetActive(false);
        objectsQueue.Enqueue(obj);
    }
}