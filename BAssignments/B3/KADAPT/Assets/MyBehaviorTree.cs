using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;

	public Transform fight1;
	public Transform fight2;

	public Transform dance1;
	public Transform dance2;	
	public Transform dance3;
	public Transform dance4;

	public Transform boxPushPoint;
	public Transform offPlatformCharacter;

	public Transform snailDest;
	
	public GameObject participant;
	public GameObject participant2;

	public GameObject snailRider;

	public GameObject danceParticipant;
	public GameObject danceParticipant2;
	public GameObject danceParticipant3;

	public SoundManager soundMan;

	int currentArc = 1;
	int currentMusic = 1;
	bool fightOccured = false;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (ST_RideSnail (snailDest));				//this.ST_PushBox(boxPushPoint, offPlatformCharacter)); //InteractiveBehaviorTree());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("space")) {

			if(currentMusic == 3)
				currentMusic = 1;
			else
				currentMusic++;

			soundMan.ChangeSongById(currentMusic);
		}
	}
	
	protected Node ST_ApproachAndWait(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}


	protected Node ST_Sit(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000),
		                     participant.GetComponent<BehaviorMecanim>().Node_Squat(), new LeafWait(1000));
	}

	protected Node ST_RideSnail(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);

		return new Sequence(snailRider.GetComponent<BehaviorMecanim> ().Node_RideSnail (),
							snailRider.GetComponent<BehaviorMecanim> ().Node_OrientTowards(position), new LeafWait(6000),
							snailRider.GetComponent<BehaviorMecanim> ().Node_NudgeTo (position, 2.0f), 
							
		                    snailRider.GetComponent<BehaviorMecanim>().Node_FallOffSnail(), new LeafWait(2000),
							snailRider.GetComponent<BehaviorMecanim> ().Node_DestroySnail ()
		                    );

	}

	protected Node ST_PushBox(Transform boxPushPoint, Transform offPlatformCharacter)
	{
		Val<Vector3> position = Val.V (() => boxPushPoint.position);
		Val<Vector3> position2 = Val.V (() => offPlatformCharacter.position);

		return new Sequence (participant.GetComponent<BehaviorMecanim> ().Node_GoTo (position), new LeafWait (1000),
		                     participant.GetComponent<BehaviorMecanim> ().Node_OrientTowards(position2), new LeafWait(100),
		                     participant.GetComponent<BehaviorMecanim>().Node_PushBox(), new LeafWait(1000),
							 participant.GetComponent<BehaviorMecanim>().Node_NudgeTo(position2, 0.3f)
		     
		                     );

	}

	protected Node ST_Breakdance(Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000),
		                    participant.GetComponent<BehaviorMecanim>().Node_Breakdance(), new LeafWait(1000));
	}

	protected Node ST_Fight(Transform target1, Transform target2)
	{
		Val<Vector3> position1 = Val.V (() => target1.position);
		Val<Vector3> position2 = Val.V (() => target2.position);
		fightOccured = true;

		return new Sequence(

							new SequenceParallel( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position1), new LeafWait(1000),
		                     					  participant2.GetComponent<BehaviorMecanim>().Node_GoTo(position2), new LeafWait(1000)),

							new SequenceParallel( participant.GetComponent<BehaviorMecanim>().Node_OrientTowards(position2), new LeafWait(1000),
		                    					  participant2.GetComponent<BehaviorMecanim>().Node_OrientTowards(position1), new LeafWait(1000)),


							new SequenceParallel( participant.GetComponent<BehaviorMecanim>().Node_FightWin(), new LeafWait(1000),
		                    					  participant2.GetComponent<BehaviorMecanim>().Node_FightLose(), new LeafWait(1000))



		                    );
	}

	protected Node ST_GatherDanceParty(Transform target1, Transform target2, Transform target3, Transform target4)
	{
		Val<Vector3> position1 = Val.V (() => target1.position);
		Val<Vector3> position2 = Val.V (() => target2.position);
		Val<Vector3> position3 = Val.V (() => target3.position);
		Val<Vector3> position4 = Val.V (() => target4.position);

		return new Sequence (

			new SequenceParallel(danceParticipant.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4), new LeafWait(1000),
		                     danceParticipant2.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4), 
		                     danceParticipant3.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4)),

			new SequenceParallel(danceParticipant.GetComponent<BehaviorMecanim>().Node_GoTo(position1), new LeafWait(1000),
		                     	danceParticipant2.GetComponent<BehaviorMecanim>().Node_GoTo(position2), 
		                     	danceParticipant3.GetComponent<BehaviorMecanim>().Node_GoTo(position3)),

			new SequenceParallel(danceParticipant.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4), new LeafWait(1000),
		                     	danceParticipant2.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4), 
		                     	danceParticipant3.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4))
		);

	}

	//TODO Rewrite with ForEach
	protected Node ST_DanceParty()
	{

		SequenceParallel danceSeq = new SequenceParallel(
			new DecoratorLoop(this.DanceByMusicID (danceParticipant)),
			new DecoratorLoop(this.DanceByMusicID (danceParticipant2)),
			new DecoratorLoop(this.DanceByMusicID (danceParticipant3)),
			new LeafTrace ("Hey")
		);


		return danceSeq;
	}

	protected Node DanceByMusicID(GameObject participant)
	{
		return new Selector (
			new Sequence (
				new LeafAssert (() => {return currentMusic == 1;}),
				new RandomNode (
					participant.GetComponent<BehaviorMecanim> ().Node_DancePolka (), new LeafWait (10)
				)
			),
			new Sequence (
				new LeafAssert (() => {return currentMusic == 2;}),
				new RandomNode (
					participant.GetComponent<BehaviorMecanim>().Node_DanceElectronic1(), new LeafWait(10),
					participant.GetComponent<BehaviorMecanim>().Node_DanceElectronic2(), new LeafWait(10)
				)
			),
			new Sequence (
				new LeafAssert (() => {return currentMusic == 3;}),
				new RandomNode (
					participant.GetComponent<BehaviorMecanim>().Node_DanceMetal1(), new LeafWait(10),
					participant.GetComponent<BehaviorMecanim>().Node_DanceMetal2(), new LeafWait(10),
					participant.GetComponent<BehaviorMecanim>().Node_DanceMetal3(), new LeafWait(10)
				)
			),
			new LeafTrace ("Default: Music not found for id")
		);

	}


	protected Node InteractiveBehaviorTree()
	{
		return new SequenceParallel (
			this.Story (),
			this.MonitorStoryState()

		);
	}

	protected Node Story()
	{
		return new DecoratorLoop (
			new Sequence (
			this.SelectStoryArc(1),
			this.SelectStoryArc(2)
			)
		);
		
	}

	protected Node SelectStoryArc(int arcID)
	{
		return new SelectorParallel (
			new DecoratorLoop(new DecoratorInvert(new LeafAssert(() => {return (currentArc == arcID);}))),

			this.SelectStoryById (arcID)

		);

	}

	protected Node SelectStoryById(int arcID)
	{
		switch (arcID) {
		case 1:
			return this.FightArc ();
		case 2:
			return this.DanceArc ();
		default:
			Debug.LogWarning("No arc selected. Default case occured.");
			return null;
		}

	}

	protected Node FightArc()
	{
		return
			new SequenceShuffle (
			this.ST_PushBox(boxPushPoint, offPlatformCharacter),
			this.ST_Fight (this.fight1, this.fight2),
			this.ST_Breakdance(this.wander2),
			this.ST_Sit(this.wander3));
	}


	protected Node DanceArc()
	{
		return

			new SequenceParallel(
				new DecoratorLoop (
						new SequenceShuffle (
							this.ST_Breakdance(this.wander2),
							this.ST_Sit(this.wander3))
					),

				new Sequence ( 
		              this.ST_GatherDanceParty(dance1, dance2, dance3, dance4),
		              this.ST_DanceParty()
				)


				);
	}


	protected Node MonitorStoryState()
	{
		while(true)
		{
			return new Selector(
					this.CheckFightArc(),
					this.CheckStandardArc()
				);
		}
	}
	
	protected Node CheckFightArc()
	{
		return new Sequence (
			new LeafAssert (() => {return !fightOccured;}),
			this.setArc(1)
			);
		
	}

	protected Node CheckStandardArc()
	{
		return new Sequence (
			new LeafAssert (() => {return fightOccured;}),
			this.setArc(2)
			);
		
	}

	protected Node setArc(int set)
	{
		this.changeArcValue (set);
		return new LeafTrace ("Arc set to " + set);
	}

	protected void changeArcValue(int set)
	{
		currentArc = set;
	}
}
