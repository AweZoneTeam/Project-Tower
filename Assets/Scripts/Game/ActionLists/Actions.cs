using UnityEngine;
using System.Collections;

public class Actions : MonoBehaviour 
{
	public ActivityClass.activites[] activities;

	void Awake()
	{
		for (int i=0;i<activities.Length;i++)
		{
			activities[i].numb=i;
			for (int j=0;j<activities[i].what.Length; j++)
				activities[i].what[j].numb=i;
		}
	}

}
