using UnityEngine;
using System.Collections;

public class OrgActions : MonoBehaviour 
{
	public OrgActivityClass.activities[] activities;

	void Awake()
	{
		for (int i=0;i<activities.Length;i++)
			activities[i].numb=i;
	}
}
