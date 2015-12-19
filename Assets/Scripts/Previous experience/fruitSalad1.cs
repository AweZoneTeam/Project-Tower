using UnityEngine;
using System.Collections;

public class fruitSalad1 : MonoBehaviour 
{
	
	void Start () 
	{
		//Notice here how the variable "myFruit" is of type
		//Fruit but is being assigned a reference to an Apple. This
		//works because of Polymorphism. Since an Apple is a Fruit,
		//this works just fine. While the Apple reference is stored 
		//in a Fruit variable; it can only be used like a Fruit
			
		Fruit1 myFruit = new Apple1 ();

		myFruit.SayHello ();
		myFruit.Chop ();

		//This is called downcasting. The variable "myFruit" which is 
		//of type Fruit, actually contains a reference to an Apple. Therefore,
		//it can safely be turned back into an Apple variable. This allows
		//it to be used like an Apple, wher before it could only used
		//like a Fruit.
		Apple1 myApple = (Apple1)myFruit;
	
		myApple.SayHello ();
		myApple.Chop ();
	}
}
