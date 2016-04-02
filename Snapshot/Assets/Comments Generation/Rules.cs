using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Rules{
	[JsonProperty(PropertyName="app_rate")]
	public int appRate;
	[JsonProperty(PropertyName="expansion")]
	public List<string> expansion;
	public ArrayList newExpansion;

	public string fires(){
		Debug.Log("fires");
		string d = "";
		for(int i = 0; i < newExpansion.Count; i++){
			if (typeof(  ).Equals (NonTerminals)) {
				foreach (Rules r in nt.rules) {
					d = d + nt.expand ();
				}
			} else {
				d = d + nt;
			}
		}
		return d;
	}
}
