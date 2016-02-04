using UnityEngine;
using System.Collections;

public class WeatherProfile {
	public Texture top;
	public Texture front;
	public Texture bottom;
	public Texture left;
	public Texture right;
	public Texture back;
	// Use this for initialization
	public WeatherProfile( Texture top_, Texture front_, Texture bottom_, Texture left_, Texture right_, Texture back_ ){
		top = top_;
		front = front_;
		bottom = bottom_;
		left = left_;
		right = right_;
		back = back_;
	}
}


