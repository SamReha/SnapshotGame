using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnBeginDrag(PointerEventData eventData){
		this.transform.position = eventData.position;
	}

	public void OnDrag(PointerEventData eventData){
		this.transform.position = eventData.position;

	}

	public void OnEndDrag(PointerEventData eventData){
		
	}
}
