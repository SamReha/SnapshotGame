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
}
