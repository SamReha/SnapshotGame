using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Rules{
	[JsonProperty(PropertyName="app_rate")]
	public int appRate;
	[JsonProperty(PropertyName="expansion")]
	public List<string> expansion;
}
