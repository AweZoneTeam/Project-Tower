using UnityEngine;
using System.Collections;

/// <summary>
/// Actor orientation. As usual, MAX is last and used to determine enum count.
/// </summary>

public class HumanoidActorActions : PersonActions
{

    #region parametres
    public bool Moving;
	public OrientationEnum MovingDirection;
	public bool TouchingGround;
	public OrientationEnum Orientation;
	public BoxCollider2D GroundCol;
	public int MovingSpeed;
	public float MovingAcceleration;
	private Rigidbody RigBody;
	public float JumpForce;
    #endregion //parametres

    #region fields
    private CharacterVisual cAnim;//Визуальная часть персонажа
    private Stats stats;//Параметры персонажа
    #endregion //fields

    public void OnCollisionEnter2D(Collision2D col) {
		TouchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		TouchingGround = false;
	}

    /// <summary>
    /// Инициализация полей и переменных
    /// </summary>
    public override void Awake ()
    {
        base.Awake();
	}

    public void Update () {
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
            if ((stats.groundness == (int)groundness.grounded)&&(cAnim!=null))
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
            if ((stats.groundness == (int)groundness.grounded)&&(cAnim!=null))
            {
                cAnim.GroundStand();
            }
        }
        if ((stats.groundness == (int)groundness.inAir)&&(cAnim!=null))
        {
            cAnim.AirMove();
        }
	}

    public override void Initialize()
    {
        cAnim = transform.FindChild("Body").gameObject.GetComponent<CharacterVisual>();
        RigBody = GetComponent<Rigidbody>();
    }

	public override void Turn(OrientationEnum Direction) {
		Vector3 newScale = this.gameObject.transform.localScale;
		newScale.x *= -1;
		if (Orientation != Direction) {
			Orientation = Direction;
			this.gameObject.transform.localScale = newScale;
		}
	}

	public override void StartWalking(OrientationEnum Direction) {
		if (!Moving) {
			Moving = true;
			MovingDirection = Direction;
		}
	}

	public override void StopWalking() {
		Moving = false;
	}

	public override void Jump() {
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

