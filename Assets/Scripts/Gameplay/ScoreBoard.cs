using UnityEngine;

public static class ScoreBoard
{
	public static int GetCurrentScore() => PlayerPrefs.GetInt("currentScore");
	public static int GetBestScore() => PlayerPrefs.GetInt("bestScore");
	public static void SetCurrentScore(int score) => PlayerPrefs.SetInt("currentScore", score);
	public static void SetBestScore(int score) => PlayerPrefs.SetInt("bestScore", score);

	public static int LossStreak = 0; 
	
	public static void UpdateScores(int winResult)
	{
		var currentScore = GetCurrentScore();
		switch (winResult)
		{
			case -1:
				SetCurrentScore(0);
				LossStreak++;
				break;
			case 0:
				break;
			case 1:
				SetCurrentScore(++currentScore);
				if (currentScore > GetBestScore())
				{
					SetBestScore(currentScore);
				}
				LossStreak = 0;
				break;
			
		}
	}
}
