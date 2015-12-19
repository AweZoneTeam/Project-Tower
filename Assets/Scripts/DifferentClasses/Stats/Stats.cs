using UnityEngine;
using System.Collections;

public class Stats : Prestats
{
	public StatsClass.stats stats;

	public void Awake()
	{
		direction = 1;
		stats.employment = 11;
	}

}
