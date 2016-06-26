using UnityEngine;
using System.Collections;

/// <summary>
/// Список действий, совершаемых падающей люстрой (и всеми объектами, которые можно сбросить)
/// </summary>
public class ChandelierActions : InterObjActions
{

    #region consts

    protected float groundRadius = 1f;

    #endregion //consts

    #region fields

    protected Rigidbody rigid;
    protected HitController hitBox;
    protected Transform groundCheck;
    protected LayerMask whatIsGround = LayerMask.NameToLayer("ground");

    public HitClass hit;

    #endregion //fields

    #region parametres

    public float minSpeed=40f;
    protected bool interactable = true;
    protected bool attack = false;
    protected bool grounded=false;

    #endregion //parametres

    public void FixedUpdate()
    {
        if (!attack)
        {
            if (rigid.velocity.y < -minSpeed)
            {
                hitBox.SetHitBox(true, hit);
                attack = true;
            }
        }
        else if (grounded)
        {
            interactable = false;
            hitBox.SetHitBox(false, hit);
            attack = false;
        }
        DefineGroundness();
    }

    public override void Initialize()
    {
        base.Initialize();
        groundCheck = transform.FindChild("GroundCheck");
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        hitBox = GetComponentInChildren<HitController>();
    }

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public override void Interact()
    {
        if (interactable)
        {
            base.Interact();
            rigid.velocity = Vector3.zero;
            rigid.isKinematic = false;
            interactable = false;
        }
    }

    /// <summary>
    /// Определить, приземлился ли объект
    /// </summary>
    protected virtual void DefineGroundness()
    {
        grounded = Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length>0;
    }



}
