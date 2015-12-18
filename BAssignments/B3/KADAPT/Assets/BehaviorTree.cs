using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;

public class BehaviorTree : MonoBehaviour {

	public GameObject bobby;
	public GameObject robert;
	public GameObject gary;

	public Transform faceForwardRight;
	public Transform faceForwardCenter;
	public Transform faceForwardLeft;

	public Transform frontRight;
	public Transform frontCenter;
	public Transform frontLeft;

	public Transform middleRight;
	public Transform middleCenter;
	public Transform middleLeft;

	public Transform backRight;
	public Transform backCenter;
	public Transform backLeft;

	public Transform offstageLeft;
	
	private bool crowdShouldCheer = true; //TODO remove this initialization

	public GameObject crowd;
	public DialogueController dialogue;
	
	private BehaviorAgent behaviorAgent;
	
	private Arcs currentArc;
	private bool introOccurred = false;
	private int countIntroduced = 0;
	
	public enum Arcs{
		Intro,
		Round1,
		Round2,
		Round3,
		Fight1,
		Award,
		Fight2
	}
	//============================
	// Use this for initialization

	void Start () {
		currentArc = Arcs.Intro;
		behaviorAgent = new BehaviorAgent (this.IntroArc());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();

	}

	protected Node InteractiveBehaviorTree() {
		return new SequenceParallel (
			CrowdReactions(),
			Story (),
			MonitorStoryState ()
		);
	}
	
	//============================
	//Monitor Story State Region

	protected Node MonitorStoryState()
	{
		while(true)
		{
			return new Selector(
				this.CheckIntroArc(),
				this.CheckRound1Arc()
				);
		}
	}
	
	protected Node CheckIntroArc()
	{
		return new Sequence (
			new LeafAssert (() => {return !introOccurred;}),
			this.setArc(Arcs.Intro)
			);
		
	}
	
	protected Node CheckRound1Arc()
	{
		return new Sequence (
			new LeafAssert (() => {return introOccurred;}),
			this.setArc(Arcs.Round1)
			);
		
	}
	
	protected Node setArc(Arcs set)
	{
		return new Sequence (
			new LeafInvoke (() => {currentArc = set;}),
			new LeafTrace ("Arc set to " + set)
		);
	}


	//============================
	//Story Method Area

	protected Node Story()
	{
		return new DecoratorLoop (
			new Sequence (
			this.SelectStoryArc(Arcs.Intro),
			this.SelectStoryArc(Arcs.Round1)
			)
			);
		
	}
	
	protected Node SelectStoryArc(Arcs arcID)
	{
		return new SelectorParallel (
			new DecoratorLoop(new LeafAssert(() => {return (currentArc != arcID);})),
			
			this.SelectStoryById (arcID)
			
			);
		
	}
	
	protected Node SelectStoryById(Arcs arcID)
	{
		switch (arcID) {
		case Arcs.Intro:
			return this.IntroArc ();
		case Arcs.Round1:
			return this.Round1Arc ();
		default:
			Debug.LogWarning("No arc selected. Default case occured.");
			return null;
		}
	}
	

	//============================
	//Monitor User Input

	protected Node MonitorUserInput(Input InputSignal)
	{
		return new DecoratorLoop (
			new Sequence(
			new LeafInvoke(() => {})


			)
		);

	}

	//Input Method Region



	//============================
	//Debug Arc

	protected Node DebugArc(){
		return new DecoratorLoop(
			new DecoratorForceStatus (RunStatus.Success,
		                          new Sequence (
			new LeafWait(1000),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The hoooooooooooooooooooly grail!", 2.0f),
			new LeafAssert (() => !crowdShouldCheer),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			new LeafWait(8000))));
	}




	//============================
	//Intro Arc

	protected Node IntroArc() 
	{
		Val<Vector3> osl = Val.V (() => offstageLeft.position);

		return new Sequence (
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Thanks for coming tonight, we've got a great show in store for you!", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The best of the best will be competing before you tonight", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Prepare to be dazzled by their amazing strength, skill, and wit!", 2.5f),

			new LeafInvoke(() => {countIntroduced = 0;}),

			new SequenceShuffle (
				this.IntroBobby (),
				this.IntroRobert (),
				this.IntroGary ()),


			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl)
			),

			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_GoTo(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_GoTo(osl),
				gary.GetComponent<BehaviorMecanim> ().Node_GoTo(osl)
			),
			
			new LeafInvoke(() => {introOccurred = true;})
			);
	}
	
	//The individual introduction events
	//Note, because these can happen in any order check if it is the last one before each
	
	protected Node IntroBobby()
	{
		Val<Vector3> fc = Val.V (() => frontCenter.position);

		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);

		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);

		 return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And last but not least...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The shady <color=#FF00>Bobby Bluth!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The shady <color=#0000BB>Bobby Bluth!</color>", 2.0f),
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(fc), new LeafWait(100),
			bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (fc), 
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "None shall stand before me.", 2.0f),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			bobby.GetComponent<BehaviorMecanim>().Node_Flex1(),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#0000BB>Bobby Bluth!</color>, everyone!", 2.0f),
			
			
			new Selector(
				new Sequence(
				new LeafAssert(() => (countIntroduced == 0)),
					bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (bl), 
					bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffl), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 1)),
					bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (bc),
					bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 2)),
					bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (br), 
					bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffr), new LeafWait(100)
				),
				new LeafTrace("This is a problem.")
			),
			
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
		
	}
	
	protected Node IntroRobert()
	{
		Val<Vector3> fc = Val.V (() => frontCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);

		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => (countIntroduced == 2)),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And last but not least...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The extra fancy <color=#FF00>Robert Redsock!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The extra fancy <color=#FF0000>Robert Redsock!</color>", 1.5f),

			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(fc), new LeafWait(100),
			robert.GetComponent<BehaviorMecanim> ().Node_GoTo (fc), 
			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "I'm GONNA' WIN!", 2.0f),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			robert.GetComponent<BehaviorMecanim>().Node_Flex2(),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "With great confidence, <color=#FF0000>Robert Redsock!</color>", 2.0f),
			
			new Selector(
				new Sequence(
				new LeafAssert(() => (countIntroduced == 0)),
					robert.GetComponent<BehaviorMecanim> ().Node_GoTo (bl), 
					robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffl), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 1)),
					robert.GetComponent<BehaviorMecanim> ().Node_GoTo (bc), 
					robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 2)),
					robert.GetComponent<BehaviorMecanim> ().Node_GoTo (br), 
					robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffr), new LeafWait(100)
				),
				new LeafTrace("This is a problem.")
			),
			
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
		
	}
	
	protected Node IntroGary()
	{
		Val<Vector3> fc = Val.V (() => frontCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => (countIntroduced == 2)),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And last but not least...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The mighty <color=#FF00>Gary Greensberg!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The mighty <color=#00CC00>Gary Greensberg!</color>", 2.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(fc), new LeafWait(100),
			gary.GetComponent<BehaviorMecanim> ().Node_GoTo (fc), 
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "I will crush the competition!", 2.0f),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			gary.GetComponent<BehaviorMecanim>().Node_Flex3(),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#00CC00>Gary Greensberg!</color>, pretty scary!", 2.0f),

			
			new Selector(
				new Sequence(
				new LeafAssert(() => (countIntroduced == 0)),
					gary.GetComponent<BehaviorMecanim> ().Node_GoTo (bl), 
					gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffl), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 1)),
					gary.GetComponent<BehaviorMecanim> ().Node_GoTo (bc), 
					gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100)
				),
				new Sequence(
				new LeafAssert(() => (countIntroduced == 2)),
					gary.GetComponent<BehaviorMecanim> ().Node_GoTo (br), 
					gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffr), new LeafWait(100)
				),
				new LeafTrace("This is a problem.")
			),

			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}








	//============================
	//Round 1 Arc
	
	
	protected Node Round1Arc() 
	{
		return new Sequence (
			new LeafInvoke(() => Debug.Log("Aw yeah."))
			);
	}
	
	
	
	
	
	
	
	
	//============================
	//Round 2 Arc
	
	
	
	
	
	
	
	
	
	
	//============================
	//Fight 1 Arc
	
	
	
	
	
	
	
	

	//============================
	//Round 3 Arc
	
	
	
	
	
	



	
	//============================
	//Award Arc
	
	
	
	
	
	


	
	//============================
	//Fight 2 Arc
	
	
	
	
	
	
	
	





	//============================	
	//Actions Section











	//============================	
	//Cheer Section

	protected Node CrowdReactions() {
		return new ForEach<GameObject> (CheerFactory, GetCrowdies ());
	}
	
	Node CheerFactory (GameObject crowdThing) {
		return new DecoratorLoop (
			new DecoratorForceStatus (RunStatus.Success,
		                          new Sequence (
			new LeafAssert (() => crowdShouldCheer),
			crowdThing.GetComponent<CrowdBehaviorMecanim> ().Cheer (),
			new LeafInvoke (() => {crowdShouldCheer = false;}))));
		
	}
	
	List<GameObject> GetCrowdies() {
		List<GameObject> ret = new List<GameObject>();
		foreach (Transform child in this.crowd.transform) {
			if(child.tag == "Crowd")
				ret.Add (child.gameObject);
		}
		return ret;
	}
}


