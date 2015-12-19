using UnityEngine;
using System.Collections;

public class LandscapeBuilder : MonoBehaviour {

	public GameObject baseObject;
	public GameObject[] buildElements;
	public float[] elementsLengthes;
	public float offset, zPosition, xposition;
	public GameObject buildBase;
	public float baseLength;
	public int koof;
	public float totalLength;

	public string[] mod;

	private SpFunctions sp;

	public void Awake () 
	{
		sp = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<SpFunctions> ();
		for (int i=0;i<mod.Length;i++)
		{
			if (mod[i]=="rope")
				MakeARope();
			if (mod[i]=="stair")
				MakeAStair();
			if (mod[i]=="up")
				CoverSurface(1,1);
			if (mod[i]=="down")
				CoverSurface(1,-1);
			if (mod[i]=="right")
				CoverSurface(2,1);
			if (mod[i]=="left")
				CoverSurface(2,-1);

		}
	}
		
	public void MakeARope ()
	{
		BoxCollider2D col;
		col=baseObject.GetComponent<BoxCollider2D>();
		float length = baseLength;
		float totalLength = Mathf.Abs (baseObject.transform.lossyScale.y)* col.size.y;
		int randomIndex=0;
		GameObject rope;
		Vector3 nextPosition;
		nextPosition = new Vector3 (baseObject.transform.position.x+xposition,
		                            baseObject.transform.position.y+totalLength/2-baseLength/2,
		                            baseObject.transform.position.z);
		while (length<totalLength)
		{
			if (length == baseLength)
			{
				rope = Instantiate(buildBase, nextPosition, buildBase.transform.rotation) as GameObject;
				nextPosition=new Vector3(nextPosition.x,
				                         nextPosition.y-baseLength/2,
				                         nextPosition.z);
			}
			else
			{
				rope = Instantiate(buildElements[randomIndex], nextPosition, buildElements[randomIndex].transform.rotation) as GameObject;
				nextPosition=new Vector3(nextPosition.x,
				                         nextPosition.y-elementsLengthes[randomIndex]/2,
				                         nextPosition.z);
			}
			randomIndex = Random.Range (0, buildElements.Length);
			length+=elementsLengthes[randomIndex];
			nextPosition=new Vector3(nextPosition.x,
			                         nextPosition.y-elementsLengthes[randomIndex],
			                         nextPosition.z);
		}
	}

	public void MakeAStair ()
	{
		BoxCollider2D col;
		col=baseObject.GetComponent<BoxCollider2D>();
		float length = baseLength;
		totalLength = Mathf.Abs (baseObject.transform.lossyScale.y)* col.size.y;
		int randomIndex=0;
		GameObject stair;
		Vector3 nextPosition;
		nextPosition = new Vector3 (baseObject.transform.position.x+xposition,
		                            baseObject.transform.position.y-totalLength/2+baseLength/2,
		                            baseObject.transform.position.z);
		while (length<totalLength)
		{
			if (length == baseLength)
			{
				stair = Instantiate(buildBase, nextPosition, buildBase.transform.rotation) as GameObject;

			}
			else
			{
				stair = Instantiate(buildElements[randomIndex], nextPosition, buildElements[randomIndex].transform.rotation) as GameObject;
			}
			randomIndex = Random.Range (0, buildElements.Length);
			length+=elementsLengthes[randomIndex];
			nextPosition=new Vector3(nextPosition.x,
			                         nextPosition.y+elementsLengthes[randomIndex],
			                         nextPosition.z);
		}
	}

	public void CoverSurface(int orientation, int dir)
	{

		BoxCollider2D col;
		GameObject surface;
		col = baseObject.GetComponent<BoxCollider2D> ();
		float totalLength;
		float length;
		float count;
		int randomIndex=0;
		Vector3 nextPosition1, nextPosition2;
		if (orientation==1)
		{
			totalLength=Mathf.Abs (baseObject.transform.lossyScale.x)* col.size.x*koof;
			length=elementsLengthes[randomIndex];
			count=sp.Div(totalLength,length);
			nextPosition1=new Vector3(baseObject.transform.position.x, 
			                          baseObject.transform.position.y+dir*Mathf.Abs (baseObject.transform.lossyScale.y)*col.size.y*koof/2-dir*offset,
			                          zPosition);
			nextPosition2=new Vector3(baseObject.transform.position.x, 
			                          baseObject.transform.position.y+dir*Mathf.Abs (baseObject.transform.lossyScale.y)*col.size.y*koof/2-dir*offset,
			                          zPosition);
			randomIndex=Random.Range(0,buildElements.Length);
			if (!sp.IsItEven(count))
			{
				surface = Instantiate(buildElements[randomIndex], nextPosition1, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,(1f-dir)*90f));
				nextPosition1=new Vector3(nextPosition1.x-length,
				                          nextPosition1.y,
				                          nextPosition1.z);
				nextPosition2=new Vector3(nextPosition2.x+length,
				                          nextPosition2.y,
				                          nextPosition2.z);

			}
			else
			{
				nextPosition1=new Vector3(nextPosition1.x-length/2,
				                          nextPosition1.y,
				                          nextPosition1.z);
				nextPosition2=new Vector3(nextPosition2.x+length/2,
				                          nextPosition2.y,
				                          nextPosition2.z);
			}
			while (Mathf.Abs(nextPosition1.x-nextPosition2.x)<totalLength)
			{
				randomIndex=randomIndex=Random.Range(0,buildElements.Length);
				surface = Instantiate(buildElements[randomIndex], nextPosition1, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,(1f-dir)*90f));
				nextPosition1=new Vector3(nextPosition1.x-length,
				                          nextPosition1.y,
				                          nextPosition1.z);
				randomIndex=randomIndex=Random.Range(0,buildElements.Length);
				surface = Instantiate(buildElements[randomIndex], nextPosition2, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,(1f-dir)*90f));
				nextPosition2=new Vector3(nextPosition2.x+length,
				                          nextPosition2.y,
				                          nextPosition2.z);

			}
		}

		if (orientation==2)
		{
			totalLength=Mathf.Abs (baseObject.transform.lossyScale.y)* col.size.y*koof;
			length=elementsLengthes[randomIndex];
			count=sp.Div(totalLength,length);
			nextPosition1=new Vector3(baseObject.transform.position.x+dir*Mathf.Abs (baseObject.transform.lossyScale.x)*col.size.x*koof/2-dir*offset, 
			                          baseObject.transform.position.y,
			                          zPosition);
			nextPosition2=new Vector3(baseObject.transform.position.x+dir*Mathf.Abs (baseObject.transform.lossyScale.x)*col.size.x*koof/2-dir*offset, 
			                          baseObject.transform.position.y,
			                          zPosition);
			randomIndex=Random.Range(0,buildElements.Length);
			if (!sp.IsItEven(count))
			{
				surface = Instantiate(buildElements[randomIndex], nextPosition1, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,270+(1f-dir)*90f));
				nextPosition1=new Vector3(nextPosition1.x,
				                          nextPosition1.y-length,
				                          nextPosition1.z);
				nextPosition2=new Vector3(nextPosition2.x,
				                          nextPosition2.y+length,
				                          nextPosition2.z);
				
			}
			else
			{
				nextPosition1=new Vector3(nextPosition1.x,
				                          nextPosition1.y-length/2,
				                          nextPosition1.z);
				nextPosition2=new Vector3(nextPosition2.x,
				                          nextPosition2.y+length/2,
				                          nextPosition2.z);
			}
			while (Mathf.Abs(nextPosition1.y-nextPosition2.y)<totalLength)
			{
				randomIndex=randomIndex=Random.Range(0,buildElements.Length);
				surface = Instantiate(buildElements[randomIndex], nextPosition1, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,270+(1f-dir)*90f));
				nextPosition1=new Vector3(nextPosition1.x,
				                          nextPosition1.y-length,
				                          nextPosition1.z);
				randomIndex=randomIndex=Random.Range(0,buildElements.Length);
				surface = Instantiate(buildElements[randomIndex], nextPosition2, buildElements[randomIndex].transform.rotation) as GameObject;
				surface.transform.Rotate(new Vector3(0f,0f,270+(1f-dir)*90f));
				nextPosition2=new Vector3(nextPosition2.x,
				                          nextPosition2.y+length,
				                          nextPosition2.z);
				
			}
		}
	}


}
