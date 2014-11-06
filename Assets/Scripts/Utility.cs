using UnityEngine;
using System.Collections;
using PathFinding;

namespace Utility
{
	public delegate void VoidEventHandler();
	public delegate void CellEventHandler(Cell cell);
	//public delegate void ObjEventHandler(Object obj);

	public static class Conversion
	{
		public static float MetersToFeet(float meters)
		{
			return meters / 0.304800610f;
		}
		
		public static float FeetToMeters(float feet)
		{
			return feet * 0.304800610f;
		}
	}
}
