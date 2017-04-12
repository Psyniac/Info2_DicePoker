using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class POOL_UPDATE : MonoBehaviour {

	public DicePoker dicePoker;
	public int current_coins;
	public float y_plus = 0.2f;
	private Vector3 start_point;
	public GameObject coin_prefab;
	// Use this for initialization
	void Start () {
		start_point = transform.position;
	}

	// Update is called once per frame
	void Update () {

		if(current_coins!=dicePoker.global_pool){
				current_coins = dicePoker.global_pool;

				UPDATE_COINS();

			}
		


	}

	public void UPDATE_COINS(){

		float current_offset_y = 0.0f;

		foreach (Transform child in transform){
			GameObject.Destroy(child.gameObject);
		}

		for (int c_index = 0; c_index < current_coins; c_index+=1 ){

				Vector3 newCPosition = new Vector3(start_point.x, start_point.y + current_offset_y, start_point.z  );
				GameObject newChild = GameObject.Instantiate(coin_prefab,newCPosition,Quaternion.identity); 
				newChild.transform.parent = transform;

				current_offset_y += y_plus;

		}
	}
}
