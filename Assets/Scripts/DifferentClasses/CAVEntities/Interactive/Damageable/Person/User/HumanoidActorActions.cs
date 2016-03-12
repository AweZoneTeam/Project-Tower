using UnityEngine;
using System.Collections;

public class HumanoidActorActions : PersonActions
{

    #region parametres

	public bool touchingGround;
	public BoxCollider2D groundCol;

    #endregion //parametres

    #region fields
    private Stats stats;//Параметры персонажа

    private WeaponClass mainWeapon;//Какое оружие персонаж носит в правой руке
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

        if (!death)
        {
            if (moving)
            {
                if (movingDirection == orientationEnum.left)
                {
                    rigid.velocity = new Vector3(-maxSpeed, rigid.velocity.y, rigid.velocity.z);
                    stats.direction = orientationEnum.left;
                }
                else
                {
                    rigid.velocity = new Vector3(maxSpeed, rigid.velocity.y, rigid.velocity.z);
                    stats.direction = orientationEnum.right;
                }
                if ((stats.groundness == groundnessEnum.grounded) && (cAnim != null))
                {
                    cAnim.GroundMove();
                }
            }
            else
            {
                if ((stats.groundness == groundnessEnum.grounded) && (rigid.velocity.x != 0))
                {
                    rigid.drag = 0.1f;
                }
                if ((stats.groundness == groundnessEnum.grounded) && (cAnim != null))
                {
                    cAnim.GroundStand();
                }
            }
            if ((stats.groundness == groundnessEnum.inAir) && (cAnim != null))
            {
                cAnim.AirMove();
            }
        }
	}

    public override void Initialize()
    {
        orientation = orientationEnum.right;
        cAnim = transform.FindChild("Body").gameObject.GetComponent<CharacterVisual>();
        rigid = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<HitController>();
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
            rigid.velocity =new Vector3(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y+jumpForce,Mathf.NegativeInfinity,jumpForce), rigid.velocity.z);
		}
	}

    /// <summary>
    /// Учёт ситуации и произведение нужной в данный момент атаки
    /// </summary>
    public override void Attack()
    {
        if ((hitData == null)&&(mainWeapon!=null))
        {
            if (stats.groundness == groundnessEnum.grounded)
            {
                Debug.Log("Kek");
                hitData = mainWeapon.GetHit("groundHit");
                if ((cAnim != null)&&(hitData!=null))
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess()); 
            }
            else if (stats.groundness == groundnessEnum.inAir)
            {
                hitData = mainWeapon.GetHit("airHit");
                if ((cAnim != null)&&(hitData!=null))
                {
                    cAnim.Attack("Hit", hitData.hitTime);
                }
                StartCoroutine(AttackProcess());
            }
        }
    }

    protected override IEnumerator AttackProcess()//Процесс атаки
    {
        if (hitBox != null)
        {
            GameObject hBox = hitBox.gameObject;
            hBox.transform.localPosition = hitData.hitPosition;
            hBox.GetComponent<BoxCollider>().size = hitData.hitSize;
            yield return new WaitForSeconds(hitData.hitTime-hitData.beginTime);
            this.hitBox.SetHitBox(hitData.beginTime - hitData.endTime, hitData);
            yield return new WaitForSeconds(hitData.beginTime);
            hitData = null;
        }
    }

    public override void GoThroughTheDoor(DoorClass door)
    {
        base.GoThroughTheDoor(door);
        SpFunctions.ChangeRoomData(door.roomPath);
        GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>().ChangeRoom();
    }

    public override void Death()
    {
        Drop();
        death = true;
        cAnim.Death();
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
    public override void SetWeapon(WeaponClass _weapon)
    {
        mainWeapon = _weapon;
    }
}

