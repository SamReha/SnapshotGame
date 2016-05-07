using UnityEngine;
using System.Collections;

public class WeatherProfile {
	public Texture top;
	public Texture front;
	public Texture bottom;
	public Texture left;
	public Texture right;
	public Texture back;
	//  New variables
	public Gradient dayNightColor;
	public float maxIntensity;
	public float minIntensity;
	public float maxAmbient;
	public float minAmbient;
	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale;
	public float dayAtmosphereThickness;
	public float nightAtmosphereThickness;


	// Use this for initialization
	public WeatherProfile( Texture top_, Texture front_, Texture bottom_, Texture left_, Texture right_, Texture back_ ,
		                   Gradient dayNightColor_, float maxIntensity_, float minIntensity_, float maxAmbient_, float minAmbient_,
		                   Gradient nightDayFogColor_, AnimationCurve fogDensityCurve_, float fogScale_, float dayAtmosphereThickness_,
		                   float nightAtmosphereThickness_ ){
		top = top_;
		front = front_;
		bottom = bottom_;
		left = left_;
		right = right_;
		back = back_;
		//  New variables
		dayNightColor = dayNightColor_;
		maxIntensity = maxIntensity_;
		minIntensity = minIntensity_ ;
		maxAmbient = maxAmbient_;
		minAmbient = minAmbient_;
		nightDayFogColor = nightDayFogColor_;
		fogDensityCurve = fogDensityCurve_;
		fogScale=fogScale_;
		dayAtmosphereThickness = dayAtmosphereThickness_;
		nightAtmosphereThickness = nightAtmosphereThickness_;
	}
}


