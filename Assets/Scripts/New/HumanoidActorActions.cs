using UnityEngine;
using System.Collections;

/// <summary>
/// Actor orientation. As usual, MAX is last and used to determine enum count.
/// </summary>

#region enums

public enum OrientationEnum {
	Left,
	Right,
	MAX
}

public enum groundness { grounded = 1, crouch, preGround, inAir };

#endregion //enums

public class HumanoidActorActions : BaseActorActions
{
	public bool Moving;
	public OrientationEnum MovingDirection;
	public bool TouchingGround;
	public OrientationEnum Orientation;
	public BoxCollider2D GroundCol;
	public int MovingSpeed;
	public float MovingAcceleration;
	private Rigidbody RigBody;
	public float JumpForce;

    private CharacterAnimator cAnim;//Визуальная часть персонажа
    private Stats stats;//Параметры персонажа
     
	public void OnCollisionEnter2D(Collision2D col) {
		TouchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		TouchingGround = false;
	}

    /// <summary>
    /// Инициализация полей и переменных
    /// </summary>
    override public void Start ()
    {
		base.Start ();
        cAnim=transform.FindChild("Body").gameObject.GetComponent<CharacterAnimator>();
        RigBody = GetComponent<Rigidbody>();
	}

	override public void Update () {
		base.Update ();
		if (Moving) {
			if (MovingDirection == OrientationEnum.Left) {
				if (MovingSpeed > RigBody.velocity.x) {
					RigBody.velocity = new Vector3 (-MovingSpeed, RigBody.velocity.y, RigBody.velocity.z);
                    stats.direction = -1;
				}
			} else {
				if (MovingSpeed > -RigBody.velocity.x) {
					RigBody.velocity = new Vector3 (MovingSpeed, RigBody.velocity.y, RigBody.velocity.z);
                    stats.direction = 1;
				}
			}
            if (stats.groundness == (int)groundness.grounded)
            {
                cAnim.GroundMove();
            }
		}
        else 
        {
            if ((stats.groundness == (int)groundness.grounded)&& (RigBody.velocity.x != 0))
            {
                RigBody.drag = 0.1f;
            }
            if (stats.groundness == (int)groundness.grounded)
            {
                cAnim.GroundStand();
            }
        }
        if (stats.groundness == (int)groundness.inAir)
        {
            cAnim.AirMove();
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
		if (stats.groundness==(int)groundness.grounded)
        {
            RigBody.AddForce(new Vector3(0f, JumpForce, 0f));
		}
	}

    public virtual void GoThroughTheDoor(DoorClass door)
    {
        
        transform.position = door.nextPosition;
        SpFunctions.ChangeRoomData(door.roomPath); 
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom();
    } 

    /// <summary>
    /// Задать поле статов
    /// </summary>
    /// <param name="задаваемые параметры"></param>
    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }
}

