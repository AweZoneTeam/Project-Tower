using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyableObjectController : NMoveableObjectController {

	public Organism stats;
	public OrgActivityClass.activities[] whatToEmploy;
	public OrgActivityClass[] whatToKek;
	public List<ActionClass.act> whatToDo; 
	public List<animClass.anim> whatToPerform;

	[HideInInspector] public int activityNumb;
	private int actNumb;

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
				if (whatToEmploy [i].what [j].actType == 0) {
					whatToDo.Add (whatToEmploy [i].what [j]);
					actNumb = whatToDo.Count;
				} else 
				{
					e=0;
					while ((whatToDo[e].actType!=whatToEmploy[i].what[j].actType)&&(e<actNumb))
						e++;
					if (e != actNumb) {
						whatToDo [e] = whatToEmploy [i].what [j];
					} else {
						whatToDo.Add(whatToEmploy [i].what [j]);
						actNumb = whatToDo.Count;
					}
				}
			}				
		}
	}

	public void LearnAnimations()
	{
		for (i=0;i<activityNumb;i++)
		{
			for (j = 0; j < whatToEmploy [i].howLook.Length; j++) {
				//animNumb = sp.AddAnimation (whatToPerform, whatToEmploy [i].howLook [j].anim, animNumb);
				whatToPerform.Add(whatToEmploy [i].howLook [j].anim);
			}
		}
		sp.SortAnimation (whatToPerform);
	}

	public void AnimateIt()
	{
		sp.BeginAnimateIt (animator.parts);
		for (i=whatToPerform.Count;i>=0;i--)
			sp.AnimateIt(animator.parts, whatToPerform[i]);
		sp.FinallyAnimateIt (animator.parts);
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
		actNumb=sp.DoActions (whatToDo);
		CoordinateActivities ();
		whatToPerform.Clear ();
		OrientateIt ();
	}
}
