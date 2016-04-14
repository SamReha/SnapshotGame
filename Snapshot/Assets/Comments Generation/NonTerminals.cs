using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NonTerminals{
	[JsonProperty(PropertyName="complete")]
	public bool complete;
	[JsonProperty(PropertyName="deep")]
	public bool deep;
	[JsonProperty(PropertyName="markup")]
	public Dictionary<string, string[]> markups;
	[JsonProperty(PropertyName="rules")]
	public List<Rules> rules;

	public string expand(){
		Random rand = new Random ();
		int rn = Random.Range (0, rules.Count-1);
		Rules rule = rules [rn];
		string s = rule.fires ();
		if (!s.Equals("")) {
				return s;
		} else {
			Debug.Log ("Empty");
		}
		return null;
	}
}
