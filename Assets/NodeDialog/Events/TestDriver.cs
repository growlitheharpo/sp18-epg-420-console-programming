using System.Collections;
using System.Collections.Generic;
using NodeDialog.Events;
using UnityEngine;

public class TestDriver : MonoBehaviour
{
	public NodeEvent mEvent;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public static void DoSomething()
	{
		Debug.Log("We're doing somethign!");
	}

	public static void DoSomethingElse()
	{
		Debug.Log("We're doing somethign elssse!!");
	}

	public static void DoSomethingWithAParameter(string param1)
	{
		Debug.Log("We're doing somethign with a param: " + param1 + "!");
	}

	public static void DoSomethingWithMultipleParams(string param1, int param2, float param3, int param4, bool param5, int param6)
	{
		Debug.Log(string.Format("Wow!! {0}  {1}  {2}  {3}  {4}", param1, param2, param3, param4, param5));
	}
}

namespace UnityEditor
{
	[CustomEditor(typeof(TestDriver))]
	public class DriverEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawDefaultInspector();
			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Test"))
			{
				((TestDriver)target).mEvent.Invoke();
			}
		}
	}

}

