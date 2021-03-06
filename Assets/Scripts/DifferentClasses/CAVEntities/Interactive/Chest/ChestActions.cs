﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Действия, совершаемые сундуком
/// </summary>
public class ChestActions : InterObjActions, ILeverActivated
{
    public LockScript chestLock;

    private ChestVisual anim;

    private BagClass chestContent;

    public override void Initialize()
    {
        anim = GetComponentInChildren<ChestVisual>();
        if (chestLock != null)
        {
            if (chestLock.lockType == 4)
            {
                chestLock = new SpecialLockScript("",false);
            }
            if (chestLock.opened)
            {
                anim.OpenedCondition();
            }
            else
            {
                anim.ClosedCondition();
            }
        }
    }

    /// <summary>
    /// Функция, которую вызовет нажатие на тот рычаг, к которому подключен сундук
    /// </summary>
    public void LeverActivation()
    {
        chestLock.opened = true;
        anim.OpenedCondition();
    }

    /// <summary>
    /// Процесс открытия сундука и его обшаривания,в зависимости от факта, закрыт ли сундук или нет?
    /// </summary>
    public override void Interact()
    {
        if (chestLock.opened)
        {
            GameObject controller = GameObject.FindGameObjectWithTag(Tags.gameController);
            BagClass bag = chestContent;
            if (interactor is PersonController)
            {
                PersonController bInteractor = (PersonController)interactor;
                EquipmentClass equip = (EquipmentClass)bInteractor.GetEquipment();
                controller.GetComponent<InterfaceController>().OpenExchangeWindow(bag,equip,bInteractor.transform, transform);
            }
        }
        else
        {
            EquipmentClass equip;
            if (interactor is PersonController)
            {
                PersonController keyControl = (PersonController)interactor;
                equip = (EquipmentClass)keyControl.GetEquipment();
                if (chestLock is SpecialLockScript)
                {
                    SpecialLockScript sChestLock = (SpecialLockScript)chestLock;
                    chestLock.opened = sChestLock.TryToOpen(equip);
                }
                else
                { 
                    chestLock.opened = chestLock.TryToOpen(equip);
                }
            }
            if (chestLock.opened)
            {
                anim.OpenedCondition();
            }
        }
    }

    public override void SetBag(BagClass _chestClass)
    {
        chestContent = _chestClass;
    }
    
}
