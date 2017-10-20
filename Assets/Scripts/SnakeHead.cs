using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeHead : MonoBehaviour
{
	public bool eat;
	public bool isDead;

	void Start ()
	{
		eat = false;
		isDead = false;
		//InvokeRepeating ("MoveHead", 0.0f, 0.05f);
	}

	public Vector2 CalcDirction (Vector2 dirctHead)
	{
		
		Vector2 pos = gameObject.transform.position;
		Collider2D[] allObjs = Physics2D.OverlapCircleAll (pos, GlobalControl.Instance.gridSize * 10);

		Vector2 foodForce = new Vector2 (0.0f, 0.0f);
		Vector2 wallForce = new Vector2 (0.0f, 0.0f);
		foreach (Collider2D obj in allObjs) {
				
			Vector2 tmpForce = GlobalControl.Instance.CalcForceVector (obj.transform.position, pos);
			if (obj.CompareTag ("Food")) {
				//Debug.Log ("---------------- Food ----------------" + tmpForce.x + "," + tmpForce.y);
				foodForce += 1000.0f * tmpForce;
			} else if (obj.CompareTag ("Block")) {
				//Debug.Log ("---------------- Board ----------------" + tmpForce.x + "," + tmpForce.y);
				wallForce -= 10.0f * tmpForce;
			} else if (obj.CompareTag ("Self")) {
				//Debug.Log ("---------------- Self ----------------" + tmpForce.x + "," + tmpForce.y);
			} else {
				//Debug.Log ("---------------- Other Body ----------------" + tmpForce.x + "," + tmpForce.y);
				//wallForce -= 2.0f * tmpForce;
			}
					
		}

		Vector2 foodDirc = GlobalControl.Instance.Force2Dircetion (foodForce);
		Vector2 wallDirc = GlobalControl.Instance.Force2Dircetion (wallForce);
		Vector2 force = GlobalControl.Instance.Force2Dircetion (OptDirction (dirctHead, wallDirc) + foodForce);
		//Debug.Log ("------------------------------------------------------------------ Finished Loop: --food, wall--------------" + foodForce + wallForce);
		
		if (force == new Vector2 (0.0f, 0.0f) || force == -dirctHead) {
			Vector2 randDirc = dirctHead + Vector2.one * (-(dirctHead.x + dirctHead.y));
			//Debug.Log ("************************Rand direction**************************" + randDirc);
				
			return randDirc;
		} else {
			//Debug.Log ("************************Calced direction**************************" + force);
			return force;	
		}
	}

	Vector2 OptDirction (Vector2 curDirc, Vector2 calcDirc) //对计算出来的方向进行优化；
	{
		if (calcDirc == new Vector2 (0.0f, 0.0f) || calcDirc == -curDirc) {
			//Debug.Log ("************************Opt Rand direction**************************");
			Vector2 randDirc = curDirc + Vector2.one * (-(curDirc.x + curDirc.y));
			randDirc *= Random.Range (0.0f, 2.0f) < 1.0f ? 1 : -1;
			return randDirc;	
		} else
			return calcDirc;
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		//print ("Collision happend\n");
		if (other.gameObject.tag == "Food") {

			if (0 == other.gameObject.name.CompareTo ("Food")) { //死亡蛇的身体;
				//print ("-------------------" + other.gameObject.name);
				Destroy (other.gameObject);
			} else //普通食物；
				other.gameObject.SetActive (false);
			//other.gameObject.SendMessage ("GetScore", 5);
			/*Destroy (other.gameObject);*/
			eat = true;

			//print ("Current score is: " + score.ToString () + "\n");
		} else {
			isDead = true;
		}
	}


}
