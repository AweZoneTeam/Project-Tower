using System;
using System.Collections.Generic;


public class ActionQueue
{
	public ActionClass next;
	public ActionClass turn;
	public ActionClass startWalk;
    

	public void AddToQueue(ActionClass action)
	{
		if(action.func == "Turn")
		{
			turn = action;
		}
		else if(action.func == "StartWalking")
		{
			startWalk = action;
		}
		else
		{
			if(next!=null)
			{
				if(next.func != "Flip")
				{
					if(next.func == "Jump" && action.func == "Flip")
					{
						next = action;
					}
					if(next.func == "Attack" && (action.func == "Flip" || action.func == "Jump"))
					{
						next = action;
					}
				}
			}
			else
			{
				next = action;
			}
		}
	}
    public void Clear()
    {
        next = turn = startWalk = null;
    }
}

