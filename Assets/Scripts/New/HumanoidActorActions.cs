using UnityEngine;
using System.Collections;

/// <summary>
/// Actor orientation. As usual, MAX is last and used to determine enum count.
/// </summary>

public enum OrientationEnum {
	Left,
	Right,
	MAX
}

public class HumanoidActorActions : BaseActorActions
{
	public bool Moving;
	public OrientationEnum MovingDirection;
	public bool TouchingGround;
	public OrientationEnum Orientation;
	public BoxCollider2D GroundCol;
	public int MovingSpeed;
	public float MovingAcceleration;
	public Rigidbody2D RigBody;
	public int JumpSpeed;

	public void OnCollisionEnter2D(Collision2D col) {
		TouchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		TouchingGround = false;
	}

	override public void Start () {
		base.Start ();
	}

	override public void Update () {
		base.Update ();
		if (Moving) {
			if (MovingDirection == OrientationEnum.Left) {
				if (MovingSpeed > RigBody.velocity.x) {
					RigBody.velocity = new Vector2 (-MovingSpeed, RigBody.velocity.y);
				}
			} else {
				if (MovingSpeed > -RigBody.velocity.x) {
					RigBody.velocity = new Vector2 (MovingSpeed, RigBody.velocity.y);
				}
			}
		} else if (RigBody.velocity.x != 0) {
			RigBody.drag = 0.1f;
		}
	}

	public virtual void Turn(OrientationEnum Direction) {
		Vector3 newScale = this.gameObject.transform.localScale;
		newScale.x *= -1;
		if (Orientation != Direction) {
			Orientation = Direction;
			this.gameObject.transform.localScale = newScale;
		}
	}

	public virtual void StartWalking(OrientationEnum Direction) {
		if (!Moving) {
			Moving = true;
			MovingDirection = Direction;
		}
	}

	public virtual void StopWalking() {
		Moving = false;
	}

	public virtual void Jump() {
		if (TouchingGround) {
			RigBody.velocity = new Vector2 (RigBody.velocity.x, JumpSpeed);
		}
	}
}

