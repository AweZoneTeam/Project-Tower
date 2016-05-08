using UnityEngine;
using System.Collections;

/// <summary>
/// Набор действий, совершаемых паукообразным обыкновенным
/// </summary>
public class SpiderActions : AIActions
{

    #region consts 

    protected const float climbOffset = 4f;//Насколько сместится паук, когда он взберётся на другую поврхность
    protected const float climbTime = 1f;//Сколько секунд паук взбирается на поверхность
    protected const float climbDelta = 0.5f;//Величина, используемая для "прилипания" паука к поверхности

    #endregion //consts

    #region parametres

    protected GroundOrientation grOrientation;

    #endregion //parametres

    #region fields

    protected Transform groundCheck;
    [SerializeField] protected LayerMask whatIsGround = LayerMask.GetMask("ground", "door");

    #endregion //fields

    public override void Initialize()
    {
        base.Initialize();
        groundCheck = transform.FindChild("Indicators").FindChild("GroundCheck");
    }

    public virtual void SetGroundOrientation(GroundOrientation _grOrientation)
    {
        grOrientation = _grOrientation;
    }

    /// <summary>
    /// Поменять поверхность, на в которой будет передвигаться паук
    /// </summary>
    public virtual void ChangeSurface(groundOrientationEnum _grEnum)
    {
        if (_grEnum != groundOrientationEnum.down)
        {
            rigid.useGravity = false;
        }
        else
        {
            rigid.useGravity = true;
        }
        transform.eulerAngles = new Vector3(0f, 0f, SpFunctions.GetAngleOfSurfaceOrientation(_grEnum));
        grOrientation.grOrientation = _grEnum;  
    }

    /// <summary>
    /// Совершить прыжок или спрыгнуть с поверхности
    /// </summary>
    public override void Jump()
    {
        if (grOrientation.grOrientation == groundOrientationEnum.down)
        {
            base.Jump();
        }
        else
        {
            ChangeSurface(groundOrientationEnum.down);
            transform.eulerAngles=Vector3.zero;
        }
    }

    /// <summary>
    /// Пресследовать цель
    /// </summary>
    public override void Pursue()
    {
        if (grOrientation.grOrientation == groundOrientationEnum.down)
        {
            if (target.position.x != transform.position.x)
            {
                StartWalking((orientationEnum)(SpFunctions.RealSign(target.position.x - transform.position.x)));
            }
        }
        else
        {
            StartWalking(movingDirection);
        }
    }

    /// <summary>
    /// Убежать от цели
    /// </summary>
    public override void Escape()
    {
        if (grOrientation.grOrientation == groundOrientationEnum.down)
        {
            if (target.position.x != transform.position.x)
            {
                StartWalking((orientationEnum)(-1*SpFunctions.RealSign(target.position.x - transform.position.x)));
            }
        }
        //else if (grOrientation.grOrientation == groundOrientationEnum.up)
        //{
          //  if (target.position.x != transform.position.x)
            //{
              //  StartWalking((orientationEnum)(SpFunctions.RealSign(target.position.x - transform.position.x)));
            //}
        //}
        else
        {
            StartWalking(movingDirection);
        }
    }

    /// <summary>
    /// Начать передвижение в заданную сторону
    /// </summary>
    public override void StartWalking(orientationEnum _direction)
    {
        if (!moving)
        {
            moving = true;
        }
        Turn(_direction);

        if (grOrientation.grOrientation == groundOrientationEnum.down)
        {
            //На нижней горизонтальной поверхности он может перепрыгивать препятствия
            if (!precipiceIsForward)
            {
                Move(_direction, runSpeed);
            }

            if (precipiceIsForward)
            {
                if (jumpIsPossible)
                {
                    if ((envStats.groundness == groundnessEnum.grounded) && (!jumped))
                    {
                        Jump();
                    }
                }
                else
                {
                    StopWalking();
                }
            }
        }

        else
        {
            Move(_direction, thicketSpeed);
        }

        movingDirection = _direction;

    }

    /// <summary>
    /// Функция, описывающая процесс передвижения
    /// </summary>
    protected override void Move(orientationEnum _direction, float _speed)
    {
        Vector3 targetVelocity = new Vector3(0f, 0f, 0f);

        targetVelocity = SpFunctions.GetSurfaceRightDirection(grOrientation.grOrientation) * (int)_direction*_speed;
            
        if (Vector3.Distance(rigid.velocity, targetVelocity) < velEps)
        {
            rigid.velocity = targetVelocity;
        }
        else
        {
            rigid.velocity = Vector3.Lerp(rigid.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Приостановиться
    /// </summary>
    public override void StopWalking()
    {
        moving = false;

        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);

        if (grOrientation.grOrientation != groundOrientationEnum.down)
        {
            rigid.velocity = new Vector3(0f, 0f, 0f);
        }
    }

    /// <summary>
    /// Обогнуть препятствие
    /// </summary>
    public override void AvoidLowObstacle(float height)
    {
        if (employment == maxEmployment)
        {
            StopWalking();
            StartCoroutine(ClimbProcess(climbTime));
        }
    }

    //Процесс взбирания на определённую поверхность
    protected virtual IEnumerator ClimbProcess(float _climbTime)
    {
        employment = maxEmployment - 1;
        yield return new WaitForSeconds(_climbTime);

        #region lowObstcl

        Vector3 eAngles = transform.eulerAngles;

        if (envStats.obstacleness == obstaclenessEnum.lowObstcl)
        {
            transform.eulerAngles = new Vector3(eAngles.x, eAngles.y, eAngles.z + (int)direction.dir * 90);
            ChangeSurface(SpFunctions.GetNextSurfaceOrientation(grOrientation.grOrientation, direction.dir==orientationEnum.left));                
        }

        #endregion //lowObstcl

        #region edge

        else if (envStats.interaction == interactionEnum.edge)
        {
            transform.position += (int)direction.dir * climbOffset * SpFunctions.GetSurfaceRightDirection(grOrientation.grOrientation);
            transform.eulerAngles = new Vector3(eAngles.x, eAngles.y, eAngles.z - (int)direction.dir*90);
            ChangeSurface(SpFunctions.GetNextSurfaceOrientation(grOrientation.grOrientation, direction.dir==orientationEnum.right));
        }

        #endregion //edge

        transform.position += (int)direction.dir*climbOffset * SpFunctions.GetSurfaceRightDirection(grOrientation.grOrientation);
        Vector3 climbDirection = (groundCheck.position - transform.position).normalized;
        Vector3 climbDestination = Vector3.zero;
        while (!(Physics.OverlapSphere(groundCheck.position + climbDestination, climbDelta, whatIsGround).Length > 0) && (climbDestination.magnitude < 10f))
        {
            climbDestination += climbDirection * climbDelta;
        }
        climbDestination += climbDirection * climbDelta;
        transform.position += climbDestination;

        employment = maxEmployment;
        envStats.interaction = interactionEnum.noInter;
        envStats.obstacleness = obstaclenessEnum.noObstcl;
    }

}
