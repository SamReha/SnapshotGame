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
		JsonConvert.PopulateObject (json, comment);
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
			parseExpansion (word, rule);
		}
	}

	void parseExpansion(string word, Rules rule){
		/*IEnumerable<string> words = SplitAndKeep (word, delimiters);
		int length = 0;
		foreach (String w in words) {
			string x = w;
			x = x+x;
			length++;
		}
		foreach (String w in words) {
			if (w != "[" && w != "]") {
				if (length == 3) {
					rule.newExpansion.Add(w);
				} else if (length == 5) {
					NonTerminals test = new NonTerminals ();
					comment.nonTerms.TryGetValue (w, out test);
					rule.newExpansion.Add(test);
				} else {
					rule.newExpansion.Add(w);
				}
			}
		}*/

		if (word.Contains ("[[")) {
			char[] t = { '[', ']' };
			word = word.Trim (t);
			NonTerminals test = new NonTerminals ();
			comment.nonTerms.TryGetValue (word, out test);
			rule.newExpansion.Add(test);
		} else if (word.Contains ("[")) {
			char[] t = { '[', ']' };
			word = word.Trim (t);
			rule.newExpansion.Add(word);
		} else {
			rule.newExpansion.Add(word);
		}
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

	public string GenerateComment( string markup ){
		foreach (NonTerminals nt in comment.nonTerms.Values) {
			string[] stuff = new string[1];
			nt.markups.TryGetValue ("Score", out stuff); 
			if (stuff != null) {
				Debug.Log (markup);
				foreach (string s in stuff) {
					if (s.Equals (markup)) {
						return nt.expand ();
					}
				}
			}
		}
		return null;
	}
}

