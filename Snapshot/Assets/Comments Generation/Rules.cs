using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Rules{
	[JsonProperty(PropertyName="app_rate")]
	public int appRate;
	[JsonProperty(PropertyName="expansion")]
	public List<string> expansion;
	public ArrayList newExpansion = new ArrayList();

	public string fires(){
		string d = "";
		for(int i = 0; i < newExpansion.Count; i++){
			if (expansion[i].Contains("[[")) {
				d = d + (newExpansion[i] as NonTerminals).expand ();
			} else {
				d = d + (newExpansion[i] as string);
			}
		}
		return d;
	}
}
