using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartConroller : MonoBehaviour 
{
	
	
	public int numb, type, mod, animationMod, frame, addictiveFrame;
	public GAF.Core.GAFMovieClip mov;
	public int realNumb;
	
	public string currentState, nextState;
	public bool loop;
	public bool reload;
	public bool led;
	[HideInInspector]
	public bool isWeaponFx;
	public int FPS;
	public int right;
	public float isItRight;
	public List<PartConroller> parts;
	public int kk;
	
	public int partsNumb;
	public SATClass.sat[] sats;
	
	private AnimationInterpretator interp;
	private SpFunctions sp;
	private SoundManager sManager;
	public AudioSource efxSource;
	private uint k = 1;
	
	public int jj;
	
	public void Awake () 
	{
		partsNumb = 0;
		interp = GetComponent<AnimationInterpretator> ();	
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		sManager=GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SoundManager> ();
	}
	
	public void Work()
	{
		isItRight = right*Mathf.Sign(transform.lossyScale.x);
		if (gameObject.GetComponent<WeaponClass> () == null) 
		{
			jj = 0;
			realNumb = numb;
		}
		else if (gameObject.GetComponent<WeaponClass>().type ==0)
		{
			jj = 0;
			realNumb = numb;
		}
		//nextState = interp.animms [type].anims [numb].rsequence;
		else 
		{
			jj = interp.animms [type].anims.Length;
			if (gameObject.GetComponent<WeaponClass>().handEmployment==2)
			isItRight=right;
			realNumb= gameObject.GetComponent<WeaponClass>().active==false? 
				numb: realNumb=interp.animms[type].anims.Length-numb-1;
			/*interp.animms [type].anims [interp.animms [type].anims.Length-numb-1].rsequence:
					interp.animms[type].anims[numb].rsequence;*/
		}
		if (isWeaponFx)
			realNumb = interp.animms [type].anims.Length - numb - 1;

		if (isItRight >= 0)
			nextState = interp.animms [type].anims [realNumb].rsequence;
		else
			nextState = interp.animms [type].anims [realNumb].lsequence;
		for (int i=0; i<parts.Count; i++)
			if (parts [i].gameObject.GetComponent<WeaponClass> () != null)
				if ((parts [i].gameObject.GetComponent<WeaponClass> ().active)&&
				    ((parts [i].gameObject.GetComponent<WeaponClass> ().orientation == isItRight)&&
				 	 (parts[i].gameObject.GetComponent<WeaponClass> ().handEmployment!=2)||
				    (parts [i].gameObject.GetComponent<WeaponClass> ().orientation == right) &&
				    (parts[i].gameObject.GetComponent<WeaponClass> ().handEmployment==2)))
					if (!parts[i].gameObject.GetComponent<AnimationInterpretator>().animms[type].anims[parts[i].gameObject.GetComponent<AnimationInterpretator>().animms[type].anims.Length-numb-1].notWeaponMove)
						nextState = parts [i].nextState;
		if ((nextState!=currentState)&&(nextState!="StopAnimation"))
		{	
			currentState=nextState;
			loop=interp.animms[type].anims[numb].loop;
			FPS=interp.animms[type].anims[numb].FPS;
			sats=interp.animms [type].anims [numb].rsats;
			mov.setSequence(currentState,true);
			mov.settings.targetFPS=k*(uint)FPS;
			if (loop)
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Loop;
			else
				mov.settings.wrapMode=GAF.Core.GAFWrapMode.Once;
			mov.setPlaying(true);
			mov.play();
		}
		if (interp.animms [type].anims [numb].stepByStep)
			mov.setPlaying(true);
		if (interp.animms [type].anims [numb].stopStepByStep)
			mov.gotoAndStop(mov.getCurrentFrameNumber());
		if (!led) 
			mov.setPlaying (true);
		else 
			mov.setPlaying(false);
		frame = (int)mov.getCurrentFrameNumber ();

		if (addictiveFrame>-1)
		{
			sp.ss=mov.name;
			mov.gotoAndStop(mov.currentSequence.startFrame+k*(uint)addictiveFrame);
			addictiveFrame=-1;
		}
		for (int i=0;i< interp.animms [type].anims [numb].rsats.Length;i++)
		{
			if ((interp.animms [type].anims [numb].rsats[i].time<=frame)&&
			    (!interp.animms [type].anims [numb].rsats[i].played))
			{
				sManager.RandomizeSfx(efxSource, interp.animms [type].anims [numb].rsats[i].audios);
				interp.animms [type].anims [numb].rsats[i].played=true;
			}
			else if ((interp.animms [type].anims [numb].rsats[i].time >frame)&&
			         (interp.animms [type].anims [numb].rsats[i].played))
			{
				interp.animms [type].anims [numb].rsats[i].played=false;					
			}
		}
		if (isItRight >= 0) {
			for (int i=0; i<interp.animms[type].anims[realNumb].taos.Length; i++)
				if ((frame >= interp.animms [type].anims [realNumb].taos [i].time) && (interp.animms [type].anims [realNumb].taos [i].order != mov.settings.spriteLayerValue))
					sp.ChangeRenderOrder (interp.animms [type].anims [realNumb].taos [i].order, mov.gameObject);
		}
		else 
		{
			for (int i=0; i<interp.animms[type].anims[realNumb].ltaos.Length; i++)
				if ((frame >= interp.animms [type].anims [realNumb].ltaos [i].time) && (interp.animms [type].anims [realNumb].ltaos [i].order != mov.settings.spriteLayerValue))
					sp.ChangeRenderOrder (interp.animms [type].anims [realNumb].ltaos [i].order, mov.gameObject);
		}
	}
}
