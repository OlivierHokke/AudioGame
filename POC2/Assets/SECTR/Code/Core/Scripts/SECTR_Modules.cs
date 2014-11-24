// Copyright (c) 2014 Make Code Now! LLC

using UnityEngine;
using System;

public static class SECTR_Modules
{
	public static bool AUDIO = false;
	public static bool VIS = false;
	public static bool STREAM = false;
	public static bool LOGIC = false;
	public static bool DEV = false;
	public static string VERSION = "1.1.3";

	static SECTR_Modules()
	{
		AUDIO = Type.GetType("SECTR_AudioSystem") != null;
		VIS = Type.GetType("SECTR_Culler") != null;
		STREAM = Type.GetType("SECTR_Chunk") != null;
		LOGIC = false; // COMING SOON!
		DEV = Type.GetType("SECTR_Tests") != null;
	}

	public static bool HasPro()
	{
		#if UNITY_4_0
		return false; // 4.0 and below users, set this to true or false based on what you have.
		#else
		return Application.HasProLicense();
		#endif
	}

	public static bool HasComplete()
	{
		return AUDIO && VIS && STREAM;
	}
}