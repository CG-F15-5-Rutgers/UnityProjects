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
	public Transform faceForwardOsr3;

	public Transform frontRight;
	public Transform frontCenter;
	public Transform frontLeft;

	public Transform middleRight;
	public Transform middleCenter;
	public Transform middleLeft;

	public Transform backRight;
	public Transform backCenter;
	public Transform backLeft;

	public Transform backLeftBelow;

	public Transform offstageLeft;
	public Transform offstageLeft2;
	public Transform offstageLeft3;
	
	public Transform offstageRight;
	public Transform offstageRight2;
	public Transform offstageRight3;

	
	private bool crowdShouldCheer = true; //TODO remove this initialization
	private bool youSuckAtPressingSpace = true;
	private bool youAreInControl = false;

	public GameObject crowd;
	public DialogueController dialogue;
	public SpacebarController spacebar;
	public CharSelect charSelect;

	private BehaviorAgent behaviorAgent;

	private Arcs currentArc;
	private bool introOccurred = false;
	private bool round1Occurred = false;
	private bool startRound2 = false;
	private int countIntroduced = 0;

	private string tempString = "ayy"; //because it will be needed for messing with dialogue
	private int bobbyScore;
	private int garyScore;
	private int robertScore;

	private int bobbyScore2;
	private int garyScore2;
	private int robertScore2;
	
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
		behaviorAgent = new BehaviorAgent (this.InteractiveBehaviorTree());
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
		return new DecoratorLoop(
			new Selector(
				this.CheckIntroArc(),
				this.CheckRound1Arc(),
				this.CheckRound2Arc()
				));
		
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
			new LeafAssert (() => {return !round1Occurred;}),
			this.setArc(Arcs.Round1)
			);
		
	}

	protected Node CheckRound2Arc()
	{
		return new Sequence (
			new LeafInvoke(() => {Debug.Log("gi");}),
			new LeafAssert (() => {return round1Occurred;}),
			this.setArc(Arcs.Round2)
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
			this.SelectStoryArc(Arcs.Round1),
			this.SelectStoryArc(Arcs.Round2)
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
		case Arcs.Round2:
			return this.Round2Arc ();
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
		Val<Vector3> osl2 = Val.V (() => offstageLeft2.position);
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);

		return new Sequence (
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Thanks for coming tonight, we've got a great show in store for you!", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The best of the best will be competing before you tonight", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Prepare to be dazzled by their amazing strength, skill, and wit!", 2.5f),

			new LeafInvoke(() => {countIntroduced = 0;}),

			new SequenceShuffle (
				this.IntroBobby (),
				this.IntroRobert (),
				this.IntroGary ()),
				

			new LeafWait(1000),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "A round of applesauce for our bold contestants, everyone!", 2.0f),
			new LeafInvoke(() => {crowdShouldCheer = true;}),

			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl)
			),
			
			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_GoTo(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_GoTo(osl2),
				gary.GetComponent<BehaviorMecanim> ().Node_GoTo(osl3)
			),

			new LeafInvoke(() => {introOccurred = true;})
			);
	}
	
	//The individual introduction events
	//Note, because these can happen in any order check if it is the last one before each
	
	protected Node IntroBobby()
	{
		Val<Vector3> fc = Val.V (() => middleCenter.position);

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
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
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
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "I'M GONNA' WIN!", 2.0f),
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
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
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
		Val<Vector3> osr = Val.V (() => offstageRight.position);
		Val<Vector3> osr2 = Val.V (() => offstageRight2.position);
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);
		
		Val<Vector3> osl = Val.V (() => offstageLeft.position);
		Val<Vector3> osl2 = Val.V (() => offstageLeft2.position);
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);

		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);

		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		
		return new Sequence (
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Our first round is a dance contest.", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The contestants have been practicing all week.", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Wish them luck!", 2.5f),


			//Move chars out to right of stage
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),



			new LeafInvoke (() => {
			countIntroduced = 0;}),

			//Each chars act
			new SequenceShuffle (
				this.Round1Bobby (),
				this.Round1Robert (),
				this.Round1Gary ()),

			//Move chars to stage for 'scoring'
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			new SequenceParallel (
				new Sequence (
					bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (bl),
					bobby.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffl)),
				new Sequence (
					robert.GetComponent<BehaviorMecanim> ().Node_GoTo (bc),
					robert.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffc)),
				new Sequence (
					gary.GetComponent<BehaviorMecanim> ().Node_GoTo (br),
					gary.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffr)),

				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Let's get our contestants out to be scored for this round.", 2.0f)),

			//Score Bobby
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#0000BB>Bobby Bluth</color>, your performance on the dancefloor awards you " + bobbyScore + " points!";}, 1.6f)),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "Hmph.", 1.0f),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => bobbyScore > 7),
					new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "No need to be so modest, <color=#0000BB>Bobby</color>", 1.6f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return bobbyScore + " points is a solid performance!";}, 1.8f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "Hmph.", 1.0f))),
			new LeafWait(400),

			//Score Gary
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#00CC00>Gary</color>, your dance nets you " + garyScore + " points!";}, 1.6f)),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => garyScore > 7),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "An impressive showing!", 1.2f),
					new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "I will crush the weak!", 1.2f))),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => garyScore < 4),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "WHAT!", 0.6f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, () => {
						if(garyScore == 1)
							return "ONLY " + garyScore + " POINT!?";
						else
							return "ONLY " + garyScore + " POINTS!?";
						}, 1.2f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "THAT'S IMPOSSIBLE!", 1.2f))),


			//Score Robert
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#FF0000>Robert</color>, your style and grace nets you " + robertScore + " points!";}, 1.6f)),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => robertScore > 7),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "You were a crowd favorite!", 1.2f),
					new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, () => {return "Only " + robertScore + " points?";}, 1.2f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "That's a very good score Robert.", 1.2f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "Remember everyone - I AM THE BEST!", 1.2f))),

				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => robertScore < 4),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "...", 0.6f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, () => {
						if(robertScore <= 1)
							return "*sniffles* hmrppphhwwaahhh... :(";
						else
							return "Oh jeeze, only " + robertScore + " points?";
						}, 2.0f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "STOP YOUR SADNESS OR I'LL PHYSICALLY BEAT YOU.", 2.0f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "... okay. D':", 1.2f))),

			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Alright everyone prepare for the next round!", 1.2f),

			
			new SequenceParallel(
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl)
			),
			
			new SequenceParallel(
			bobby.GetComponent<BehaviorMecanim> ().Node_GoTo(osl),
			robert.GetComponent<BehaviorMecanim> ().Node_GoTo(osl2),
			gary.GetComponent<BehaviorMecanim> ().Node_GoTo(osl3)
			),

			new LeafInvoke(() => {Debug.Log ("gahh");}),
			new LeafInvoke(() => {round1Occurred = true;}),
			
			new LeafInvoke(() => {startRound2 = true;})
			
			);
	}
	
	protected Node Round1Bobby()
	{
		Val<Vector3> fc = Val.V (() => frontCenter.position);
		Val<Vector3> fr = Val.V (() => frontRight.position);
		Val<Vector3> fl = Val.V (() => frontLeft.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);

		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);

		Val<Vector3> osl = Val.V (() => offstageLeft.position);

		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final dance act...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#0000BB>Bobby Bluth</color>, show me what you got!", 2.0f),
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(bc), new LeafWait(100),
			bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (bc), 
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(100),

			new LeafInvoke(()=> {youAreInControl = (charSelect.active == Char.Gary);}),
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(()=> youAreInControl),
				new LeafInvoke(()=> {youSuckAtPressingSpace = true;}),
				spacebar.GetComponent<SpacebarBehaviorMecanim>().PressSpaceOrLose(),
				new LeafInvoke(()=> {youSuckAtPressingSpace = false;}))),



			bobby.GetComponent<BehaviorMecanim> ().Node_DanceMetal1(),
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (fc, 0.3f), new LeafWait(1000),
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (bl, 1.0f),
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (br, 1.0f),
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (fc, 1.0f),
			new LeafWait(200),
			
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "Boo.", 1.5f),

			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => youAreInControl),
				new LeafAssert(() => !youSuckAtPressingSpace),
				new LeafInvoke(() => {crowdShouldCheer = true;}),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Nice timing!", 0.6f),
				new LeafInvoke(() => {bobbyScore = 10;}))),
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => youAreInControl),
				new LeafAssert(() => youSuckAtPressingSpace),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch,", 0.6f),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch, that didn't look so good", 0.6f),
				new LeafInvoke(() => {bobbyScore = 0;}))),

			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => !youAreInControl),
				new LeafInvoke(() => {crowdShouldCheer = true;}))),
				

			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl), new LeafWait(100),
			bobby.GetComponent<BehaviorMecanim> ().Node_GoTo (osl),
			
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);

	}
	
	protected Node Round1Gary()
	{
		Val<Vector3> fc = Val.V (() => frontCenter.position);
		Val<Vector3> fr = Val.V (() => frontRight.position);
		Val<Vector3> fl = Val.V (() => frontLeft.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);

		Val<Vector3> osl2 = Val.V (() => offstageLeft2.position);
		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final dance act...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Let's see your moves <color=#FF00>Gary Greensberg!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Let's see your moves <color=#00CC00>Gary Greensberg!</color>", 2.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(fc), new LeafWait(100),
			gary.GetComponent<BehaviorMecanim> ().Node_GoTo (fc), 
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffc), new LeafWait(200),
			
			gary.GetComponent<BehaviorMecanim> ().Node_DanceElectronic1(),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (bc, 1.5f),// new LeafWait(200),
			gary.GetComponent<BehaviorMecanim> ().Node_DanceElectronic1(),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (fc, 1.0f), //new LeafWait(500),

			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl2), new LeafWait(100),


			new SequenceParallel(
			new Sequence(
			new LeafWait(2000),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "...", 0.7f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "... I guess that's that!", 1.2f)),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			gary.GetComponent<BehaviorMecanim> ().Node_GoTo (osl2)
			),
			
			new LeafWait(200),

			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}

	protected Node Round1Robert()
	{
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);

		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);
		Val<Vector3> ffosr3 = Val.V (() => faceForwardOsr3.position);
		
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);

		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final dance act...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Go get em' <color=#FF00>Robert Redsock!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Go get em' <color=#FF0000>Robert Redsock!</color>", 2.0f),

			new SequenceParallel(
				new Sequence(
					robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(ffosr3), new LeafWait(100),
					robert.GetComponent<BehaviorMecanim> ().Node_Breakdance(),
					new DecoratorForceStatus(RunStatus.Success, new Sequence(
						new LeafInvoke(()=> {youSuckAtPressingSpace = true;}),
						spacebar.GetComponent<SpacebarBehaviorMecanim>().PressSpaceOrLose(),
						new LeafInvoke(()=> {youSuckAtPressingSpace = false;}))),
					robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osl3, 5.5f)),
				new Sequence(
					new LeafWait(1600),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "Eat my dust.", 2.3f))),
			
			new LeafWait(200),
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => !youSuckAtPressingSpace),
				new LeafInvoke(() => {crowdShouldCheer = true;}),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Nice timing!", 0.6f),
				new LeafInvoke(() => {robertScore = 10;}))),
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => youSuckAtPressingSpace),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch,", 0.6f),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch, that didn't look so good", 0.6f),
				new LeafInvoke(() => {robertScore = 0;}))),

			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}


	
	
	
	
	
	//============================
	//Round 2 Arc
	
	
	protected Node Round2Arc() 
	{
		Val<Vector3> osr = Val.V (() => offstageRight.position);
		Val<Vector3> osr2 = Val.V (() => offstageRight2.position);
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);
		
		Val<Vector3> osl = Val.V (() => offstageLeft.position);
		Val<Vector3> osl2 = Val.V (() => offstageLeft2.position);
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);

		Val<Vector3> blb = Val.V (() => backLeftBelow.position);

		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		
		return new Sequence (
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "The second round is a bit...", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Extreme.", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Goodluck contestants.", 2.5f),


			//Move chars out to right of stage
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			
			
			
			new LeafInvoke (() => {
			countIntroduced = 0;}),
			
			//Each chars act
			new SequenceShuffle (
			this.Round2Bobby (),
			this.Round2Robert (),
			this.Round2Gary ()),


			//Move chars to stage for 'scoring'
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osr, 0.0f),
			
			new SequenceParallel (
				new Sequence (
					bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (bl, 2.0f),
					bobby.GetComponent<BehaviorMecanim>().Node_DismountSnail(), new LeafWait(1000)
					),
				new Sequence (
					robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (bc, 2.0f), 
					robert.GetComponent<BehaviorMecanim>().Node_DismountSnail(), new LeafWait(1000)
					),
				new Sequence (
					gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (br, 2.0f), 
					gary.GetComponent<BehaviorMecanim>().Node_DismountSnail(), new LeafWait(1000)
					),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Let's get our contestants out one more time to be scored for this round.", 2.0f)
			),

			new LeafWait(1800),

			new SequenceParallel(
				new Sequence(
					bobby.GetComponent<BehaviorMecanim>().Node_DestroySnail(), new LeafWait(1000),
					bobby.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffl)),
				
				new Sequence(
					robert.GetComponent<BehaviorMecanim>().Node_DestroySnail(), new LeafWait(1000),
					robert.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffc)),

				new Sequence(
					gary.GetComponent<BehaviorMecanim>().Node_DestroySnail(), new LeafWait(1000),
					gary.GetComponent<BehaviorMecanim>().Node_OrientTowards(ffr)
				)
			),
			
			new LeafInvoke(() => {
				garyScore2= Random.Range (1,11);
				if(garyScore2 == robertScore2) garyScore2--;
			}),

			new LeafInvoke(() => {
				bobbyScore2= Random.Range (2,11);
				if(bobbyScore2 == robertScore2) bobbyScore2--;
				if(bobbyScore2 == garyScore2) bobbyScore2--;
			}),
			
			//Score Bobby
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#0000BB>Bobby Bluth</color>, your sweet trick earned you " + bobbyScore2 + " points!";}, 1.6f)),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "Hmph.", 1.0f),
				
				new DecoratorForceStatus(RunStatus.Success,  
			        new Sequence(
						new LeafAssert(() => bobbyScore2 > 7),
						new LeafInvoke(() => {crowdShouldCheer = true;}),
						dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Aren't you a little excited <color=#0000BB>Bobby?</color>", 1.6f),
						dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return bobbyScore2 + " points is two big thumbs way up!";}, 2.8f),
						dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "*SCCREEEAACH*", 1.0f)
						)
	        ),
			
			
			new LeafWait(400),
			
			//Score Gary
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#00CC00>Gary</color>, flipping us off gets you " + garyScore2 + " points!";}, 1.6f)),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => garyScore2 > 7),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Quiet happy with your score, aren't you?", 1.2f),
				new LeafInvoke(() => {crowdShouldCheer = true;}),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "I need to get huge, I need to stay huge.", 1.2f),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "I NEED TO GET HUGE, I NEED TO STAY HUGE", 1.2f),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "BUILD MASS AND LEAN BULK.", 1.2f)

			)),
			
				new DecoratorForceStatus(RunStatus.Success, 
		           new Sequence(
					new LeafAssert(() => garyScore2 < 4),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "WHAT!", 0.6f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, () => {
					if(garyScore == 1)
						return "ONLY " + garyScore2 + " POINT!?";
					else
						return "ONLY " + garyScore2 + " POINTS!?";
					}, 1.2f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "NO WAY - THIS CONTEST IS A SCAM!", 1.2f))),
			
			
			//Score Robert
			new Sequence (
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, () => {return "<color=#FF0000>Robert</color>, " + robertScore2 + " points!";}, 1.6f)),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => robertScore2 > 7),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "What are you feeling right now?", 1.2f),
					new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, () => {return "I feel like " + robertScore2 + " bucks.";}, 1.2f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "I'm glad to hear!", 1.2f))),
			
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => robertScore2 < 4),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "...", 0.6f),
				dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, () => {
				if(robertScore2 == 1)
					return "Oh noooo!";
				else
					return ":(. " + robertScore2 + " points?";
				}, 1.2f)
			)),
				


			new LeafInvoke(() =>
            {
				robertScore += robertScore2;
				garyScore += garyScore2;
				bobbyScore += bobbyScore2;
			}),


			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "SOoooo, the moment we've all been waiting for!", 2.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Our winner is...", 2.5f),



			new Selector(
				new Sequence(
					new LeafAssert(() => (robertScore > garyScore && robertScore > bobbyScore)),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#FF0000>ROBERT REDSOCK</color>", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "You've been awarded...", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "A lifetime supply of Holy Grails!!!!!", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),
					
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "What are you going to do now?", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "Take game science next semester!", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "What are you talking about?", 2.5f)
			),
				new Sequence(
					new LeafAssert(() => (garyScore > robertScore && garyScore > bobbyScore)),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#00CC00>GARY GREENBURG</color>", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "You've been awarded...", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "A lifetime supply of Holy Grails!!!!!", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),
			
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "What are you going to do now?", 2.5f),

			
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "Eat some meat.", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Nice!", 2.5f)
			),
				new Sequence(
					new LeafAssert(() => (bobbyScore > garyScore && bobbyScore > robertScore)),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "<color=#0000BB>BOBBY BLUTH</color>", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "You've been awarded...", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "A lifetime supply of Holy Grails!!!!!", 2.5f), new LeafInvoke(() => {crowdShouldCheer = true;}),

					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "What are you going to do now?", 2.5f),

					
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "*FFFFFFFFFFFFFFFFFFFFFF*", 2.5f),
					bobby.GetComponent<BehaviorMecanim>().Node_NudgeTo(blb, 6.0f),
					bobby.GetComponent<BehaviorMecanim>().Node_NudgeTo(osl, 0.0f),
			
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Interesting!", 2.5f)
			),
				new Sequence(
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "No one! There was a tie.", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Go home.", 2.5f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "All of you.", 2.5f)
					
				)
			),
				
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "That concludes our talent show! See you cats later. I'M OUT! PEACE!", 2.5f),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			
			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl),
				gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl)
			),
			
			new SequenceParallel(
				bobby.GetComponent<BehaviorMecanim> ().Node_GoTo(osl),
				robert.GetComponent<BehaviorMecanim> ().Node_GoTo(osl2),
				gary.GetComponent<BehaviorMecanim> ().Node_GoTo(osl3)
			),
			
			new LeafInvoke(() => {round1Occurred = true;}),
			new LeafTrace("End.")

			
			);
	}
	
	protected Node Round2Robert()
	{
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);
		Val<Vector3> ffosr3 = Val.V (() => faceForwardOsr3.position);
		
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);
		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final extreme stunt of the night...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Give em' hell <color=#FF00>Robert Redsock!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Give em' hell <color=#FF0000>Robert Redsock!</color>", 2.0f),
			
			new SequenceParallel(
			new Sequence(
			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(br), new LeafWait(100),
			robert.GetComponent<BehaviorMecanim> ().Node_GoTo(br), new LeafWait(100),

			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3),
			robert.GetComponent<BehaviorMecanim> ().Node_RideSnail (),

			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafInvoke(()=> {youSuckAtPressingSpace = true;}),
				spacebar.GetComponent<SpacebarBehaviorMecanim>().PressSpaceOrLose(),
				new LeafInvoke(()=> {youSuckAtPressingSpace = false;}))),

			robert.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3), new LeafWait(100),
			robert.GetComponent<BehaviorMecanim> ().Node_SnailTrick1(),
			robert.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osl3, 5.5f)),
			new Sequence(
			new LeafWait(1600),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Robert, "Check me out.", 2.6f))),
			
			new LeafWait(200),
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
				new LeafAssert(() => !youSuckAtPressingSpace),
					new LeafInvoke(() => {crowdShouldCheer = true;}),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Nice timing!", 0.6f),
					new LeafInvoke(() => {robertScore2 = 10;}))),
				new DecoratorForceStatus(RunStatus.Success, new Sequence(
					new LeafAssert(() => youSuckAtPressingSpace),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch,", 0.6f),
					dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Ouch, that didn't look so good", 0.6f),
					new LeafInvoke(() => {robertScore2 = 0;}))),
					
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}
	
	protected Node Round2Gary()
	{
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);
		Val<Vector3> ffosr3 = Val.V (() => faceForwardOsr3.position);
		
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);
		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final extreme stunt of the night...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Show them what you are made of <color=#FF00>Gary Greenburg!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Show them what you are made of <color=#00CC00>Gary Greenburg!</color>", 2.0f),
			
			new SequenceParallel(
			new Sequence(
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(br), new LeafWait(100),
			gary.GetComponent<BehaviorMecanim> ().Node_GoTo(br), new LeafWait(100),
			
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3),
			gary.GetComponent<BehaviorMecanim> ().Node_RideSnail (),
			gary.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3), new LeafWait(100),
			gary.GetComponent<BehaviorMecanim> ().Node_SnailTrick2(),
			gary.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osl3, 5.5f)),
			new Sequence(
			new LeafWait(1600),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Gary, "I WILL FLIP YOU ALL OFF YOUR SEATS.", 2.6f))),
			
			new LeafWait(200),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}	
	
	protected Node Round2Bobby()
	{
		Val<Vector3> fc = Val.V (() => middleCenter.position);
		
		Val<Vector3> bl = Val.V (() => backLeft.position);
		Val<Vector3> bc = Val.V (() => backCenter.position);
		Val<Vector3> br = Val.V (() => backRight.position);
		
		Val<Vector3> ffl = Val.V (() => faceForwardLeft.position);
		Val<Vector3> ffc = Val.V (() => faceForwardCenter.position);
		Val<Vector3> ffr = Val.V (() => faceForwardRight.position);
		
		Val<Vector3> osl3 = Val.V (() => offstageLeft3.position);
		Val<Vector3> ffosr3 = Val.V (() => faceForwardOsr3.position);
		
		Val<Vector3> osr3 = Val.V (() => offstageRight3.position);
		
		return new Sequence (
			new DecoratorForceStatus(RunStatus.Success, new Sequence(
			new LeafAssert(() => (countIntroduced == 2)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "And our final extreme stunt of the night...", 1.5f)
			)),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Do your thing <color=#FF00>Bobby Bluth!</color>", 0.5f),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Host, "Do your thing <color=#0000BB>Bobby Bluth!</color>", 2.0f),
			
			new SequenceParallel(
			new Sequence(
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(br), new LeafWait(100),
			bobby.GetComponent<BehaviorMecanim> ().Node_GoTo(br), new LeafWait(100),
			
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3),
			bobby.GetComponent<BehaviorMecanim> ().Node_RideSnail (),
			bobby.GetComponent<BehaviorMecanim> ().Node_OrientTowards(osl3), new LeafWait(100),
			bobby.GetComponent<BehaviorMecanim> ().Node_SnailTrick3(),
			bobby.GetComponent<BehaviorMecanim> ().Node_NudgeTo (osl3, 5.5f)),
			new Sequence(
			new LeafWait(1600),
			dialogue.GetComponent<DialogueBehaviorMecanim>().Speak(Speaker.Bobby, "*CLICK* *CLICK*", 2.3f))),
			
			new LeafWait(200),
			new LeafInvoke(() => {crowdShouldCheer = true;}),
			
			new LeafInvoke(() => {countIntroduced += 1;}),
			new LeafWait(2000)
			);
	}	
	
	
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


