using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CommentGenerator : MonoBehaviour {

	string json;
	public Comments comment = new Comments();
	char[] delimiters = { "[[", "]]", '[', ']' };


	// Use this for initialization
	void Start () {
		json = System.IO.File.ReadAllText(@Application.dataPath+"/Comments Generation/Comments.json");
		Debug.Log (json);
		JsonConvert.PopulateObject (json, comment);
		foreach (KeyValuePair<string, NonTerminals> test in comment.nonTerms) {
			Debug.Log (test.Key);
			Debug.Log (test.Value.complete);
			Debug.Log (test.Value.deep);
			foreach (KeyValuePair<string, string[]> x in test.Value.markups) {
				Debug.Log (x.Key);
				Debug.Log (x.Value [0]);
			}
			Debug.Log (test.Value.rules[0].appRate);
			foreach (string s in test.Value.rules[0].expansion) {
				Debug.Log (s);
			}
		}
		foreach (NonTerminals nt in comment.nonTerms.Values) {
			parseNonTerminals (nt);
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void parseNonTerminals (NonTerminals nt) {
		foreach (Rules rule in nt.rules) {
			parseRules (rule);	
		}
	}

	void parseRules(Rules rule){
		foreach (string word in rule.expansion) {
			parseExpansion (word);
		}
	}

	void parseExpansion(string word){
		string[] words = word.Split (delimiters);
			if (words.Length != 1) {
				if (words.Length != 3) {

				} else {

				}
			}
	}
}

