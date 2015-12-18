using UnityEngine;
using TreeSharpPlus;
using System;
using System.Collections;
using System.Collections.Generic;

using RootMotion.FinalIK;

public class CharacterMecanim : MonoBehaviour
{
    public const float MAX_REACHING_DISTANCE = 1.1f;
    public const float MAX_REACHING_HEIGHT = 2.0f;
    public const float MAX_REACHING_ANGLE = 100;

    private Dictionary<FullBodyBipedEffector, bool> triggers;
    private Dictionary<FullBodyBipedEffector, bool> finish;

	public GameObject Snail1;
	
    [HideInInspector]
    public BodyMecanim Body = null;
	

    void Awake() { this.Initialize(); }

    /// <summary>
    /// Searches for and binds a reference to the Body interface
    /// </summary>
    public void Initialize()
    {
        this.Body = this.GetComponent<BodyMecanim>();
        this.Body.InteractionTrigger += this.OnInteractionTrigger;
        this.Body.InteractionStop += this.OnInteractionFinish;
    }

    private void OnInteractionTrigger(
        FullBodyBipedEffector effector, 
        InteractionObject obj)
    {
        if (this.triggers == null)
            this.triggers = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.triggers.ContainsKey(effector))
            this.triggers[effector] = true;
    }

    private void OnInteractionFinish(
        FullBodyBipedEffector effector,
        InteractionObject obj)
    {
        if (this.finish == null)
            this.finish = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.finish.ContainsKey(effector))
            this.finish[effector] = true;
    }

    #region Smart Object Specific Commands
    public virtual RunStatus WithinDistance(Vector3 target, float distance)
    {
        if ((transform.position - target).magnitude < distance)
            return RunStatus.Success;
        return RunStatus.Failure;
    }

    public virtual RunStatus Approach(Vector3 target, float distance)
    {
        Vector3 delta = target - transform.position;
        Vector3 offset = delta.normalized * distance;
        return this.NavGoTo(target - offset);
    }
    #endregion

    #region Navigation Commands
    /// <summary>
    /// Turns to face a desired target point
    /// </summary>
    public virtual RunStatus NavTurn(Val<Vector3> target)
    {
        this.Body.NavSetOrientationBehavior(OrientationBehavior.None);
        this.Body.NavSetDesiredOrientation(target.Value);
        if (this.Body.NavIsFacingDesired() == true)
        {
            this.Body.NavSetOrientationBehavior(
                OrientationBehavior.LookForward);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    /// <summary>
    /// Turns to face a desired orientation
    /// </summary>
    public virtual RunStatus NavTurn(Val<Quaternion> target)
    {
        this.Body.NavSetOrientationBehavior(OrientationBehavior.None);
        this.Body.NavSetDesiredOrientation(target.Value);
        if (this.Body.NavIsFacingDesired() == true)
        {
            this.Body.NavFacingSnap();
            this.Body.NavSetOrientationBehavior(
                OrientationBehavior.LookForward);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

	/// <summary>
	/// Sets a custom orientation behavior
	/// </summary>
	public virtual RunStatus NavOrientBehavior(
		Val<OrientationBehavior> behavior)
	{
		this.Body.NavSetOrientationBehavior(behavior.Value);
        return RunStatus.Success;
    }

    /// <summary>
    /// Sets a new navigation target. Will fail immediately if the
    /// point is unreachable. Blocks until the agent arrives.
    /// </summary>
    public virtual RunStatus NavGoTo(Val<Vector3> target)
    {
        if (this.Body.NavCanReach(target.Value) == false)
        {
			Debug.Log ("IVE FAILED!");
            return RunStatus.Failure;
        }
        // TODO: I previously had this if statement here to prevent spam:
        //     if (this.Interface.NavTarget() != target)
        // It's good for limiting the amount of SetDestination() calls we
        // make internally, but sometimes it causes the character1 to stand
        // still when we re-activate a tree after it's been terminated. Look
        // into a better way to make this smarter without false positives. - AS
        this.Body.NavGoTo(target.Value);
        if (this.Body.NavHasArrived() == true)
        {
            this.Body.NavStop();
            return RunStatus.Success;
        }
        return RunStatus.Running;
        // TODO: Timeout? - AS
    }

    /// <summary>
    /// Lerps the character towards a target. Use for precise adjustments.
    /// </summary>
    public virtual RunStatus NavNudgeTo(Val<Vector3> target, float time)
    {
        bool? result = this.Body.NavDoneNudge();
        if (result == null)
        {
            this.Body.NavNudge(target.Value, time);
        }
        else if (result == true)
        {
            this.Body.NavNudgeStop();
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }



	public virtual RunStatus NavSquat()
	{
		if(this.Body.IsStanding())
			NavSit ();

		if (this.Body.NavHasSat ()) {
			NavStand ();
			return RunStatus.Success;
		}

		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavPushBox()
	{
		if(!this.Body.isPushingBox())
			this.Body.PushBox (true);
		
		if (this.Body.NavHasPushedBox ()) {
			this.Body.PushBox (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavRideSnail()
	{
		if (!this.Body.isRidingSnail ()) {
			this.Body.RideSnail (true);
			Snail1 = Instantiate(Snail1) as GameObject;
			Snail1.transform.parent = this.Body.transform;
		}

		return RunStatus.Success;
		
		//return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavDismountSnail()
	{
		if (this.Body.isRidingSnail ()) {
			this.Body.RideSnail (false);
		}
		
		return RunStatus.Success;
		
		//return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavDestroySnail()
	{
		Destroy(Snail1, 1);
		return RunStatus.Success;
	}

	public virtual RunStatus NavBreakdance()
	{
		if(!this.Body.isBreakdancing())
			this.Body.Breakdance (true);
		
		if (this.Body.NavHasBreaked ()) {
			this.Body.Breakdance (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavFallOffSnail()
	{
		if(!this.Body.isFallingOffSnail())
			this.Body.FallOffSnail (true);

		
		Snail1.transform.Translate(Vector3.right * Time.deltaTime);
		
		if (this.Body.NavHasFellOffSnail ()) {
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}


	public virtual RunStatus NavSnailTrick1()
	{
		if(!this.Body.isSnailTrick1ing())
			this.Body.SnailTrick1 (true);
		
		if (this.Body.NavHasSnailTrick1ed ()) {
			this.Body.SnailTrick1 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavSnailTrick2()
	{
		if(!this.Body.isSnailTrick2ing())
			this.Body.SnailTrick2 (true);
		
		if (this.Body.NavHasSnailTrick2ed ()) {
			Debug.Log("FUCKK.");
			this.Body.SnailTrick2 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavSnailTrick3()
	{
		if(!this.Body.isSnailTrick3ing())
			this.Body.SnailTrick3 (true);
		
		if (this.Body.NavHasSnailTrick3ed ()) {
			this.Body.SnailTrick3 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavPolka()
	{
		if(!this.Body.isPolkaing())
			this.Body.Polka (true);
		
		if (this.Body.NavHasPolkaed ()) {
			this.Body.Polka (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}


	public virtual RunStatus NavElectronic1()
	{
		if(!this.Body.isElectronic1ing())
			this.Body.Electronic1 (true);
		
		if (this.Body.NavHasElectronic1ed ()) {
			this.Body.Electronic1 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavElectronic2()
	{
		if(!this.Body.isElectronic2ing())
			this.Body.Electronic2 (true);
		
		if (this.Body.NavHasElectronic2ed ()) {
			this.Body.Electronic2 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavMetal1()
	{
		if(!this.Body.isMetal1ing())
			this.Body.Metal1 (true);
		
		if (this.Body.NavHasMetal1ed ()) {
			this.Body.Metal1 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavMetal2()
	{
		if(!this.Body.isMetal2ing())
			this.Body.Metal2 (true);
		
		if (this.Body.NavHasMetal2ed ()) {
			this.Body.Metal2 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavMetal3()
	{
		if(!this.Body.isMetal3ing())
			this.Body.Metal3 (true);
		
		if (this.Body.NavHasMetal3ed ()) {
			this.Body.Metal3 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}


	
	public virtual RunStatus NavFlex1()
	{
		if(!this.Body.isFlex1ing())
			this.Body.Flex1 (true);
		
		if (this.Body.NavHasFlex1ed ()) {
			this.Body.Flex1 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}
	
	public virtual RunStatus NavFlex2()
	{
		if(!this.Body.isFlex2ing())
			this.Body.Flex2 (true);
		
		if (this.Body.NavHasFlex2ed ()) {
			this.Body.Flex2 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}
	
	public virtual RunStatus NavFlex3()
	{
		if(!this.Body.isFlex3ing())
			this.Body.Flex3 (true);
		
		if (this.Body.NavHasFlex3ed ()) {
			this.Body.Flex3 (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}


	public virtual RunStatus NavFightWin()
	{
		this.Body.FistsUp (true);
		this.Body.fightWin (true);
		
		if (this.Body.NavHasFoughtWin ()) {
			this.Body.FistsUp (false);
			this.Body.fightWin (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavFightLose()
	{
		if (this.Body.getDead() == true)
		{
			return RunStatus.Failure;
		}

		this.Body.FistsUp (true);
		this.Body.fightLose (true);
		
		if (this.Body.NavHasFoughtLose ()) {
			this.Body.setDead(true);
			this.Body.FistsUp (false);
			this.Body.fightLose (false);
			return RunStatus.Success;
		}
		
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}

	public virtual RunStatus NavSit()
	{
		if (this.Body.NavHasSat() == true)
		{
			return RunStatus.Failure;
		}
		
		this.Body.SitDown();
		if (this.Body.IsSitting() == true)
		{
			return RunStatus.Success;
		}
		return RunStatus.Running;
		// TODO: Timeout? - AS




		/*
		if (this.Body.NavCanReach(target.Value) == false)
		{
			Debug.LogWarning("NavGoTo failed -- can't reach target");
			return RunStatus.Failure;
		}
		// TODO: I previously had this if statement here to prevent spam:
		//     if (this.Interface.NavTarget() != target)
		// It's good for limiting the amount of SetDestination() calls we
		// make internally, but sometimes it causes the character1 to stand
		// still when we re-activate a tree after it's been terminated. Look
		// into a better way to make this smarter without false positives. - AS
		this.Body.NavGoTo(target.Value);
		if (this.Body.NavHasArrived() == true)
		{
			this.Body.NavStop();
			return RunStatus.Success;
		}
		return RunStatus.Running;
		// TODO: Timeout? - AS
*/



	}

	public virtual RunStatus NavStand()
	{
		if (this.Body.IsSitting() == false)
		{
			return RunStatus.Failure;
		}
		
		this.Body.StandUp();
		if (this.Body.NavHasSat() == true)
		{
			this.Body.NavStop();
			return RunStatus.Success;
		}
		return RunStatus.Running;
		// TODO: Timeout? - AS
	}



    private IEnumerator<RunStatus> snapper;

    private RunStatus TickSnap(
        Vector3 position,
        Vector3 target,
        float time = 1.0f)
    {
        if (this.snapper == null)
            this.snapper = 
                SnapToTarget(position, target, time).GetEnumerator();
        if (this.snapper.MoveNext() == false)
        {
            this.snapper = null;
            return RunStatus.Success;
        }
        return snapper.Current;
    }

    private IEnumerable<RunStatus> SnapToTarget(
        Vector3 position,
        Vector3 target,
        float time)
    {
        Interpolator<Vector3> interp =
            new Interpolator<Vector3>(
                position,
                target,
                Vector3.Lerp);
        interp.ForceMin();
        interp.ToMax(time);

        while (interp.State != InterpolationState.Max)
        {
            transform.position = interp.Value;
            yield return RunStatus.Running;
        }
        yield return RunStatus.Success;
        yield break;
    }

	/// <summary>
	/// Stops the Navigation system. Blocks until the agent is stopped.
	/// </summary>
	public virtual RunStatus NavStop()
    {
        this.Body.NavStop();
        if (this.Body.NavIsStopped() == true)
            return RunStatus.Success;
        return RunStatus.Running;
        // TODO: Timeout? - AS
    }
    #endregion

    #region Interaction Commands
    public virtual RunStatus WaitForTrigger(
        Val<FullBodyBipedEffector> effector)
    {
        if (this.triggers == null)
            this.triggers = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.triggers.ContainsKey(effector.Value) == false)
            this.triggers.Add(effector.Value, false);
        if (this.triggers[effector.Value] == true)
        {
            this.triggers.Remove(effector.Value);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    public virtual RunStatus WaitForFinish(
        Val<FullBodyBipedEffector> effector)
    {
        if (this.finish == null)
            this.finish = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.finish.ContainsKey(effector.Value) == false)
            this.finish.Add(effector.Value, false);
        if (this.finish[effector.Value] == true)
        {
            this.finish.Remove(effector.Value);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    public virtual RunStatus StartInteraction(
        Val<FullBodyBipedEffector> effector, 
        Val<InteractionObject> obj)
    {
        this.Body.StartInteraction(effector, obj);
        return RunStatus.Success;
    }

    public virtual RunStatus ResumeInteraction(
        Val<FullBodyBipedEffector> effector)
    {
        this.Body.ResumeInteraction(effector);
        return RunStatus.Success;
    }

    public virtual RunStatus StopInteraction(Val<FullBodyBipedEffector> effector)
    {
        this.Body.StopInteraction(effector);
        return RunStatus.Success;
    }	
    #endregion

    #region HeadLook Commands
    public virtual RunStatus HeadLookAt(Val<Vector3> target)
    {
        this.Body.HeadLookAt(target);
        return RunStatus.Success;
    }

    public virtual RunStatus HeadLookStop()
    {
        this.Body.HeadLookStop();
		return RunStatus.Success;
	}
    #endregion

    #region Animation Commands
    public virtual RunStatus FaceAnimation(
        Val<string> gestureName, Val<bool> isActive)
    {
        this.Body.FaceAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}
	
	public virtual RunStatus HandAnimation(
        Val<string> gestureName, Val<bool> isActive)
    {
        this.Body.HandAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}

	public virtual RunStatus BodyAnimation(Val<string> gestureName, Val<bool> isActive)
	{
		this.Body.BodyAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}

	public RunStatus ResetAnimation()
    {
        this.Body.ResetAnimation();
        return RunStatus.Success;
    }
    #endregion

    #region Sitting Commands
    /// <summary>
    /// Sits the character down
    /// </summary>
    public virtual RunStatus SitDown()
    {
        if (this.Body.IsSitting() == true)
            return RunStatus.Success;
        this.Body.SitDown();
        return RunStatus.Running;
    }

    /// <summary>
    /// Stands the character up
    /// </summary>
    public virtual RunStatus StandUp()
    {
        if (this.Body.IsStanding() == true)
            return RunStatus.Success;
        this.Body.StandUp();
        return RunStatus.Running;
    }
    #endregion
}
