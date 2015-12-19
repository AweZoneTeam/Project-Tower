using UnityEngine;
using System.Collections;

public class DestroyableObjectController : NMoveableObjectController {

	public Organism stats;
	public OrgActivityClass.activities[] whatToEmploy;
	public ActionClass.act[] whatToDo; 
	public animClass.anim[] whatToPerform;

	[HideInInspector] public int activityNumb;
	private int actNumb;
	private int animNumb;

	private OrgActions actions;

	int i,j;


	public void LearnActivities()
	{
		j = sp.ChemByZanyatsya (actions.activities, stats);
	}

	public void LearnActions()
	{
		int e;
		for (i=0; i<activityNumb; i++)
		{
			for (j=0; j<whatToEmploy[i].what.Length;j++)
			{
				if (whatToEmploy[i].what[j].actType==0)
					actNumb=sp.AddAction(whatToDo,whatToEmploy[i].what[j],actNumb);
				else 
				{
					e=0;
					while ((whatToDo[e].actType!=whatToEmploy[i].what[j].actType)&&(e<actNumb))
						e++;
					if (e!=actNumb)
						sp.ChangeAction(whatToDo,whatToEmploy[i].what[j],e);
					else
						actNumb=sp.AddAction(whatToDo,whatToEmploy[i].what[j],actNumb);
				}
			}				
		}
	}

	public void LearnAnimations()
	{
		for (i=0;i<activityNumb;i++)
		{
			for (j=0;j<whatToEmploy[i].howLook.Length; j++)
					animNumb=sp.AddAnimation(whatToPerform, whatToEmploy[i].howLook[j].anim, animNumb);
		}
		sp.SortAnimation (whatToPerform, animNumb);
	}

	public void AnimateIt()
	{
		sp.BeginAnimateIt (animator.headParts);
		for (i=animNumb-1;i>=0;i--)
			sp.AnimateIt(animator.headParts, whatToPerform[i]);
		sp.FinallyAnimateIt (animator.headParts);
	}

	public void CoordinateActivities()
	{
		for(i=0;i<activityNumb;i++)
		{	

			if (whatToEmploy[i].timeOfAction>0)
				whatToEmploy[i].timeOfAction--;
			if (whatToEmploy[i].timeOfAction==0)
			{
				actions.activities[whatToEmploy[i].numb].chosen=false; 
				activityNumb=sp.DeleteActivity(whatToEmploy,i,activityNumb);
				i--;
			}		
		}
	}

	public void OrientateIt()
	{
		sp.Flip (transform, stats.direction);
	}

	public override void Awake () 
	{
		base.Awake ();	
		stats = GetComponent<Organism> ();
		actions = gameObject.GetComponent<OrgActions> ();
	}
	

	public void Update () 
	{
		LearnActivities ();
		LearnActions ();
		LearnAnimations ();
		AnimateIt ();
		actNumb=sp.DoActions (whatToDo, actNumb);
		CoordinateActivities ();
		animNumb = 0;

		OrientateIt ();
	}
}
