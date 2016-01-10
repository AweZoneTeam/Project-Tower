using UnityEngine;
using System.Collections;

/// <summary>
/// Base actor actions. Contain stats about self, and are able to take hit and be activated
/// </summary>

public class BaseActorActions : MonoBehaviour
{
	/// <summary>
	/// The knockback resistance. When -1 all attacks deal no knockback
	/// </summary>
	public int KnockbackResistance;
	/// <summary>
	/// The health. When -1 actor is immortal.
	public int Health;
	/// <summary>
	/// The defence. Reduces incoming damage by this percentage.
	/// </summary>
	private int[] Defence;
	// Use this for initialization
	/// <summary>
	/// The animator. Not meant to be called from outside, but can be set during initialization.
	/// </summary>
	public BaseAnimator Animator;

	virtual public void Start ()
	{
		Defence = new int[(int)AttackType.MAX];
	}
	
	// Update is called once per frame
	virtual public void Update ()
	{
	
	}
	/// <summary>
	/// Processes incoming damage
	/// </summary>
	/// <param name="Attack">Attack. Contains information about damage and knockback</param>
	public virtual void GetHit(AttackData Attack) {
		int i;
		int Damage = 0;
		if (Health > 0) {
			for (i = 0; i < (int)AttackType.MAX; i++) {
				Damage += Attack.Damage [i] * ((100 - Defence [i]) / 100);
			}
			if (Damage >= Health) {
				Die ();
			}
		}
		if (KnockbackResistance >= 0) {
			//Getting knocked back
		}
	}
	/// <summary>
	/// Kill this instance.
	/// </summary>
	public virtual void Die() {
		Destroy (this.gameObject);
	}
	/// <summary>
	/// Reaction to being activated.
	/// </summary>
	public virtual void Activated() {
		print ("I was activated!");
	}
}

