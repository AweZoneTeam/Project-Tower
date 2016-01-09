using UnityEngine;
using System.Collections;

public class Organism : Stats 
{
	public float health;
	public float prevHealth;
	public float maxHealth;
	public int addPDefence, addFDefence, addDDefence, addADefence;
	public int pDefence, fDefence, dDefence, aDefence;
	public ShieldController shield;
	public int stability;
	public float k;	
	private bool kk;


	private Color color;
	private CharacterAnimator anim;
	private SpFunctions sp;

	public void Awake()
	{
		kk = false;
		if (gameObject.GetComponent<CharacterAnimator> () != null)
			anim = gameObject.GetComponent<CharacterAnimator> ();
		else
			anim = null;
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
	}
	public void FixedUpdate()
	{

		if (!kk)
		{
			if (shield!=null)
			{
				if (shield.active)
				{
					addPDefence+=shield.pDefence;
					addFDefence+=shield.fDefence;
					addADefence+=shield.aDefence;
					addDDefence+=shield.dDefence;
					kk=true;
				}
			}

		}
		
		if (kk)
		{
			if (shield!=null)
			{
				if (!shield.active)
				{
					addPDefence=0;
					addFDefence=0;
					addADefence=0;
					addDDefence=0;
					kk=false;
				}
			}
		}

		if (shield==null)
		{
			addPDefence=0;
			addFDefence=0;
			addADefence=0;
			addDDefence=0;
			kk=false;
		}

		if (pDefence+addPDefence>100) addPDefence=100-pDefence;
		if (fDefence+addFDefence>100) addFDefence=100-fDefence;
		if (aDefence+addADefence>100) addADefence=100-aDefence;
		if (dDefence+addDDefence>100) addDDefence=100-dDefence;

		if (prevHealth < health)
		{
			prevHealth = health;
			k=0f;
		}
		else if ((prevHealth==health)&&(k<=0.1f))
		{
			stats.hitted=0;
			k=0f;
		}
		else if (prevHealth>health)
		{
			prevHealth-=1f;
			if (k<0.05f)
				k = 1f;
		}
		if (k < 0f)
		{
			k = 0f;
		}
		else if (k > 0.1f)
			k -= 0.05f;
		color = Color.red+(1f-k)*Color.green+(1f-k)*Color.blue;
		if (anim!=null)
			sp.ChangePartColor(anim.parts,color);
	}

}
