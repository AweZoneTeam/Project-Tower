using UnityEngine;
using System.Collections;

/// <summary>
/// Base charcter controller. When activated or hit, invokes corresponding actions without any conscious reaction
/// </summary>

public class BaseActorController : MonoBehaviour
{
	private BaseActorActions Actions;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public virtual void GetHit(AttackData Attack) {
		Actions.GetHit (Attack);
	}

	public virtual void Activated() {
		Actions.Activated ();
	}
}

