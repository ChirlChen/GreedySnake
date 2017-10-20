using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalControl : MonoBehaviour
{

	public static GlobalControl Instance;

	//要保存使用的数据;
	public int playerCnt;
	public float mapWidth;
	public float mapHeight;
	public Vector3 mapCenter;
	public float gridSize = 9.5f;

	//初始化
	private void Awake ()
	{
		if (Instance == null) {
			DontDestroyOnLoad (gameObject);
			Instance = this;
		} else if (Instance != null) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	public float GetRandomX ()
	{
		float x = Random.Range (-mapWidth + 20.0f, mapWidth - 20.0f) + mapCenter.x;
		x = gridSize * (int)(x / gridSize);

		return x;
	}

	public float GetRandomY ()
	{
		float y = Random.Range (-mapHeight + 20.0f, mapHeight - 20.0f) + mapCenter.y;
		y = gridSize * (int)(y / gridSize);

		return y;
	}

	public float CalcAngle (Vector2 dir1, Vector2 dir2)
	{
		//print ("(" + dir1.x.ToString () + ", " + dir1.y.ToString () + ") -> (" + dir2.x.ToString () + ", " + dir2.y.ToString () + ") ");
		return 90.0f * (dir1.x * dir2.y - dir1.y * dir2.x);
	}

	// 绑定按钮事件；
	public Button FindButton (string path)
	{
		//获取按钮游戏对象
		GameObject btnObj = GameObject.Find (path);
		//获取按钮脚本组件
		Button btn = btnObj.GetComponent<Button> ();

		return btn;
	}

	public Vector2 CalcForceVector (Vector2 pos1, Vector2 pos2)
	{
		Vector2 tmpForce = new Vector2 ((pos1.x == pos2.x) ? 0.0f : 1.0f / (pos1.x - pos2.x), 
			                   (pos1.y == pos2.y) ? 0.0f : 1.0f / (pos1.y - pos2.y));
		return tmpForce;
	}

	public Vector2 Force2Dircetion (Vector2 force)
	{
		Vector2 absForce = new Vector2 (Mathf.Abs (force.x), Mathf.Abs (force.y));
		if (absForce.x > 0.005f || absForce.y > 0.005f) {
			force.x = absForce.x >= absForce.y ? force.x / absForce.x : 0.0f;
			force.y = absForce.x < absForce.y ? force.y / absForce.y : 0.0f;
			return force;
		}
		return new Vector2 (0.0f, 0.0f);
	}

	public bool IsSmallDistanceWith (Vector2 selfPos, Vector2 tarPos)
	{
		if (Vector2.Distance (selfPos, tarPos) < 20.0f) {
			return true;
		} else
			return false;
	}

	public bool IsCloseToWall (Vector2 selfPos)
	{
		if (Mathf.Abs (Mathf.Abs (selfPos.x) - mapWidth) < 30.0f ||
		    Mathf.Abs (Mathf.Abs (selfPos.y) - mapHeight) < 40.0f)
			return true;
		else
			return false;
	}
}


