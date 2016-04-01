using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Rules{
	[JsonProperty(PropertyName="app_rate")]
	public int appRate;
	[JsonProperty(PropertyName="expansion")]
	public List<string> expansion;
	public List<NonTerminals> ruleReferences = new List<NonTerminals> ();

	public string fires(){
		string d = "";
		foreach(NonTerminals nt in ruleReferences){
			foreach(Rules r in nt.rules){
				d = d + nt.expand ();
			}
		}
		return d;
	}
}
