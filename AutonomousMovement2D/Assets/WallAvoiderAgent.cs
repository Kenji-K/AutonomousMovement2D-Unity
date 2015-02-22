using UnityEngine;
using System.Collections;
using Kensai.AutonomousMovement;

public class WallAvoiderAgent : MonoBehaviour {

	// Use this for initialization
	void Start () {
        World2D.Instance.Walls.Add(new Wall2D(new Vector2(0, 0), new Vector2(10f, 0)));
        World2D.Instance.Walls.Add(new Wall2D(new Vector2(10, 0), new Vector2(10f, 7.6f)));
        World2D.Instance.Walls.Add(new Wall2D(new Vector2(10, 7.6f), new Vector2(0, 7.6f)));
        World2D.Instance.Walls.Add(new Wall2D(new Vector2(0, 7.6f), new Vector2(0, 0)));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
