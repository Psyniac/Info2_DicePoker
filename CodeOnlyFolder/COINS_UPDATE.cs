using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class COINS_UPDATE : MonoBehaviour {

	public DicePoker dicePoker;
	public int current_coins;
	public int player_id;

	public float x_plus = 2.0f;
	public float y_plus = 0.2f;

	public int stack_counter = 0;

	private Vector3 start_point;

	public GameObject coin_prefab;
	// Use this for initialization
	void Start () {
		start_point = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if(player_id == 2){
			if(current_coins!=dicePoker.player_1_coins){
				current_coins = dicePoker.player_1_coins;

				UPDATE_COINS();

			}
		}else{
			if(current_coins!=dicePoker.player_2_coins){
				current_coins = dicePoker.player_2_coins;

				UPDATE_COINS();

			}
		}


	}

	public void UPDATE_COINS(){
		stack_counter = -1;
		float current_offset_x = 0.0f;
		float current_offset_y = 0.0f;

		foreach (Transform child in transform){
			GameObject.Destroy(child.gameObject);
		}

		for (int c_index = 0; c_index < current_coins; c_index+=1 ){
			stack_counter += 1;
			if(stack_counter == 5){
				current_offset_y = 0.0f;
				current_offset_x += x_plus;
				stack_counter = 0;
			}
				Vector3 newCPosition = new Vector3(start_point.x + current_offset_x, start_point.y + current_offset_y, start_point.z  );
				GameObject newChild = GameObject.Instantiate(coin_prefab,newCPosition,Quaternion.identity); 
				newChild.transform.parent = transform;

				current_offset_y += y_plus;

		}
	}
}
