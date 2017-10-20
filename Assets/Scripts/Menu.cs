using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		Button btn = GlobalControl.Instance.FindButton ("Begin");
		//添加点击侦听
		btn.onClick.AddListener (Begin);

		btn = GlobalControl.Instance.FindButton ("Setting");
		//添加点击侦听
		btn.onClick.AddListener (Setting);
	}



	// Update is called once per frame
	void Update ()
	{
		
	}

	//开始新游戏
	void Begin ()
	{
		SceneManager.LoadScene (1);
	}

	//游戏设置
	void Setting ()
	{
		print ("Click Setting\n");
	}
}
