using UnityEngine;
using System.Collections;

public class HumanoidActorActions : PersonActions
{

    #region parametres
	public bool touchingGround;
	public BoxCollider2D groundCol;
	public int movingSpeed;
	public float movingAcceleration;
	private Rigidbody rigBody;
	public float jumpForce;
    #endregion //parametres

    #region fields
    private CharacterVisual cAnim;//Визуальная часть персонажа
    private Stats stats;//Параметры персонажа

    private WeaponClass rightWeapon;//Какое оружие персонаж носит в правой руке
    private HitController rightHitBox;//Хитбокс оружия в правой руке
    private HitClass hitData=null;//Какими параметрами атаки персонаж пользуется в данный момент
    #endregion //fields

    public void OnCollisionEnter2D(Collision2D col) {
		touchingGround = true;
	}

	public void OnCollisionExit2D(Collision2D col) {
		touchingGround = false;
	}

    /// <summary>
    /// Инициализация полей и переменных
    /// </summary>
    public override void Awake ()
    {
        base.Awake();
	}

    public void Update () {
		if (moving) {
			if (movingDirection == orientationEnum.left)
            {
			    rigBody.velocity = new Vector3 (-movingSpeed, rigBody.velocity.y, rigBody.velocity.z);
                stats.direction =orientationEnum.left;
			}
            else
            { 
                rigBody.velocity = new Vector3 (movingSpeed, rigBody.velocity.y, rigBody.velocity.z);
                stats.direction = orientationEnum.right;
			}
            if ((stats.groundness == groundnessEnum.grounded)&&(cAnim!=null))
            {
                cAnim.GroundMove();
            }
		}
        else 
        {
            if ((stats.groundness == groundnessEnum.grounded)&& (rigBody.velocity.x != 0))
            {
                rigBody.drag = 0.1f;
            }
            if ((stats.groundness == groundnessEnum.grounded)&&(cAnim!=null))
            {
                cAnim.GroundStand();
            }
        }
        if ((stats.groundness == groundnessEnum.inAir)&&(cAnim!=null))
        {
            cAnim.AirMove();
        }
	}

    public override void Initialize()
    {
        orientation = orientationEnum.right;
        cAnim = transform.FindChild("Body").gameObject.GetComponent<CharacterVisual>();
        rigBody = GetComponent<Rigidbody>();
        rightHitBox = GetComponentInChildren<HitController>();
        hitData = null;
    }

/*	public override void Turn(OrientationEnum Direction) {
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
	}*/

	public override void Jump() {
		if (stats.groundness==groundnessEnum.grounded)
        {
            rigBody.AddForce(new Vector3(0f, jumpForce, 0f));
		}
	}

    /// <summary>
    /// Функция прохода через двери
    /// </summary>
    public virtual void GoThroughTheDoor(DoorClass door)
    { 
        transform.position = door.nextPosition;
        SpFunctions.ChangeRoomData(door.roomPath); 
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom();
    }

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
    public virtual void Attack()
    {
        if ((hitData == null)&&(rightWeapon!=null))
        {
            if (stats.groundness == groundnessEnum.grounded)
            {
                Debug.Log("Kek");
                hitData = rightWeapon.groundHit;
                if (cAnim != null)
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess()); 
            }
            else if (stats.groundness == groundnessEnum.inAir)
            {
                hitData = rightWeapon.airHit;
                if (cAnim != null)
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess());
            }
        }
    }

    IEnumerator AttackProcess()//Процесс атаки
    {
        if (rightHitBox != null)
        {
            GameObject hitBox = rightHitBox.gameObject;
            hitBox.transform.localPosition = hitData.hitPosition;
            hitBox.GetComponent<BoxCollider>().size = hitData.hitSize;
            yield return new WaitForSeconds(hitData.hitTime-hitData.beginTime);
            rightHitBox.SetHitBox(hitData.beginTime - hitData.endTime, hitData);
            yield return new WaitForSeconds(hitData.beginTime);
            hitData = null;
        }
    }

    /// <summary>
    /// Задать поле статов
    /// </summary>
    public override void SetStats(Stats _stats)
    {
        stats = _stats;
    }

    /// <summary>
    /// Установить в правой руке нужное оружие
    /// </summary>
    public void SetWeapon(WeaponClass _weapon)
    {
        rightWeapon = _weapon;
    }
}

