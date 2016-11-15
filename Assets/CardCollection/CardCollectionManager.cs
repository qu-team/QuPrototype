using UnityEngine;
using System.Collections;

public class CardCollectionManager : MonoBehaviour {
	
	GameManager gameManager;
	public GameObject[] cards;
	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		cards[0].GetComponent<CardBehaviour>().DisplayedCard = Card.Collection[1];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
