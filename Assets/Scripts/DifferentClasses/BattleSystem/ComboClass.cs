using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ComboClass
{
	public string comboName;
	public List<string> preCombo = new List<string>();//после каких комбо может идти это комбо в очереди
	public List<HitClass> hitData = new List<HitClass>();
}
