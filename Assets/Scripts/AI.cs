using UnityEngine;
using System.Collections;

public partial class AI : Unit
{
	public enum Agression { HOSTILE, PASSIVE };

	public Agression m_agression = Agression.HOSTILE;
}
