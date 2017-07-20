using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables {
	public static int round = 1;
	public static int lives = 2;
	public static int score = 0;


	public static void Reset() {
		round = 1;
		lives = 2;
		score = 0;
	}
}
