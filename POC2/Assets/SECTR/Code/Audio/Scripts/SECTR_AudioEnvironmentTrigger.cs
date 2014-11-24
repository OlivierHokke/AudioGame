// Copyright (c) 2014 Make Code Now! LLC

using UnityEngine;
using System.Collections.Generic;

/// \ingroup Audio
/// Activates a SECTR_AudioAmbience whenever the sibling
/// trigger volume is entered.
/// 
/// AudioEnvironmentTriggers activate based on the standard
/// Unity trigger events. As such, they will work with any shaped
/// collider, provided it's marked as a trigger. As with all
/// SECTR_AudioEnvironment components, AudioEnvironmentTriggers
/// can be overlapped and nested.
[AddComponentMenu("SECTR/Audio/SECTR Audio Environment Trigger")]
public class SECTR_AudioEnvironmentTrigger : SECTR_AudioEnvironment
{
	#region Private Details
	Collider activator = null;
	#endregion

	#region Unity Interface
	void OnEnable()
	{
		// If we still have an activator, they must still be in the trigger,
		// So auto-restart.
		if(activator)
		{
			Activate();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(activator == null)
		{
			Activate();
			activator = other;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(activator == other)
		{
			Deactivate();
			activator = null;
		}
	}
	#endregion
}