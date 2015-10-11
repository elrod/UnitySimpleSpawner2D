using UnityEngine;
using System.Collections;

public class CloudMovement : MonoBehaviour {

    public enum Direction { N, S, E, W };

    public float speed = 5f;
    public Direction direction = Direction.W;

    public GenericSpawner theSpawner;

	// Update is called once per frame
	void Update () {
        switch (direction)
        {
            case Direction.N:
                transform.Translate(Vector3.up * speed * Time.deltaTime);
                break;
            case Direction.S:
                transform.Translate(Vector3.down * speed * Time.deltaTime);
                break;
            case Direction.E:
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                break;
            case Direction.W:
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                break;
        }
	}
    void OnBecameInvisible()
    {
        theSpawner.RequeueObject(gameObject);
    }
}
