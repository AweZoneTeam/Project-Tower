using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : MonoBehaviour {

    #region fields

    protected List<string> enemies = new List<string>();
    protected Rigidbody rigid;

    #endregion //fields

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
	{
		if(GetComponent<Rigidbody>().velocity.magnitude>0)
		{
            transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
		}
	}

    /// <summary>
    /// Исправить срочно!!!!
    /// </summary>
	void OnTriggerEnter(Collider other)
	{
		if(enemies.Contains(other.tag))
		{
			GetComponent<RemovingObj>().lifeTime = 3;
			transform.parent = other.transform;
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

    public void SetEnemies(List<string> _enemies)
    {
        enemies = _enemies;
    }

}
