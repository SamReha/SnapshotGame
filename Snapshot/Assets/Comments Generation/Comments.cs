using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Comments{
	[JsonProperty(PropertyName = "markups")]
	public Dictionary<string, string[]> markups;
	[JsonProperty(PropertyName = "nonterminals")]
	public Dictionary<string, NonTerminals> nonTerms;
	[JsonProperty(PropertyName = "system_vars")]
	public string[] systemVars;
}
