using UnityEngine;
using System.Collections

public class ThrowObject: MonoBehavior {
  public AudioClip clip;
  
  private float throw_speed = 600f; //random magic number as a placeholder
  private AudioSource src;
  private float volumeLow = .5f; //placeholder value
  private float volumeHigh = 1.0f //placeholder value
  
  void Awake() {
    src = GetComponent<AudioSource>();
  }
  
  void Update() {
    //to be implemented
  }
}
