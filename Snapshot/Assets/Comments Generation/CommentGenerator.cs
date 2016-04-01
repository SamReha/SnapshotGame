using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class CommentGenerator : MonoBehaviour {

	string json;
	public Comments comment = new Comments();
	char[] delimiters = { '[', ']' };


	// Use this for initialization
	void Start () {
		json = System.IO.File.ReadAllText(@Application.dataPath+"/Comments Generation/Comments.json");
		//Debug.Log (json);
		JsonConvert.PopulateObject (json, comment);
		foreach (KeyValuePair<string, NonTerminals> test in comment.nonTerms) {
			//Debug.Log (test.Key);
			//Debug.Log (test.Value.complete);
			//Debug.Log (test.Value.deep);
			foreach (KeyValuePair<string, string[]> x in test.Value.markups) {
				//Debug.Log (x.Key);
				//Debug.Log (x.Value [0]);
			}
			List<Rules> r = test.Value.rules;
			if ( r.Count > 0) {
				//Debug.Log (test.Value.rules [0].appRate);
				foreach (string s in test.Value.rules[0].expansion) {
					//Debug.Log ("Expansion: " + s);
				}
			} else {
				//Debug.Log ("Empty Expansion");
			}
		}
		foreach (NonTerminals nt in comment.nonTerms.Values) {
			parseNonTerminals (nt);
			foreach (Rules r in nt.rules) {
				foreach (NonTerminals n in r.ruleReferences) {
					//Debug.Log (n);
				}
			}
		}
		GenerateComment ("bad");
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
			rule.ruleReferences = parseExpansion (word);
		}
	}

	List<NonTerminals> parseExpansion(string word){
		List<NonTerminals> rules = new List<NonTerminals>();
		//Debug.Log ("Word: " + word);
		IEnumerable<string> words = SplitAndKeep (word, delimiters);
		//Debug.Log ("Words: ");
		int length = 0;
		foreach (String w in words) {
			//Debug.Log (w);
			length++;
		}
		foreach (String w in words) {
			if (w != "[" && w != "]") {
				if (length == 3) {
					//Debug.Log ("Sysvar");
					//words [1];
				} else if (length == 5) {
					NonTerminals test = new NonTerminals ();
					comment.nonTerms.TryGetValue (w, out test);
					rules.Add (test);
					//Debug.Log ("Rule");
				} else {
					//Debug.Log ("Word");
				}
			}
		}
		return rules;
	}
   
   public static IEnumerable<string> SplitAndKeep( string s, char[] delims)
    {
        int start = 0, index;

        while ((index = s.IndexOfAny(delims, start)) != -1)
        {
            if(index-start > 0)
                yield return s.Substring(start, index - start);
            yield return s.Substring(index, 1);
            start = index + 1;
        }

        if (start < s.Length)
        {
            yield return s.Substring(start);
        }
    }

	void GenerateComment( string markup ){
		foreach (NonTerminals nt in comment.nonTerms.Values) {
			string[] stuff = new string[1];
			nt.markups.TryGetValue ("Score", out stuff); 
			Debug.Log ("Test");
			if (stuff [0].Equals (markup)) {
				string d = nt.expand ();
				Debug.Log (d);
			}
		}

	}




}

