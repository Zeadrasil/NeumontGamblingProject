using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
	[SerializeField] private Wheel[] wheels;

	[Header("UI")]
	[SerializeField] private GameObject startMenu;
	[SerializeField] private TMP_Text betDisplay;
	[SerializeField] private TMP_Text winningsDisplay;
	[SerializeField] private TMP_Text creditsDisplay;

	[Header("Events")]
	[SerializeField] private EventChannel onStartGame;
	[SerializeField] private EventChannel onStopGame;
	[SerializeField] private EventChannel onSpinStart;
	[SerializeField] private IntEventChannel onSpinResult;
	[SerializeField] private EventChannel onSpinResultDone;
	[SerializeField] private IntEventChannel onUpdateBet;

	[Header("Audio")]
	[SerializeField] private AudioClipEvent onAudioEvent;
	[SerializeField] private AudioClip cashoutAudioClip;
	[SerializeField] private AudioClip creditsAudioClip;
	[SerializeField] private AudioClip betupAudioClip;
	[SerializeField] private AudioClip betdownAudioClip;
	[SerializeField] private AudioClip startSpinAudioClip;
	[SerializeField] private AudioClip stopSpinAudioClip;
		
	private int state = -2;
	private float spinTimeLeft = 0.5f;
	private int currentWheel = 0;
	public float currentSpin = 0;
	public float expectedReturnModifier = 1;

	private float reward;
	private int credits = 0;
	private int bet = 0;
	private int winnings = 0;

	private void Start()
	{
		onSpinResultDone.Subscribe(OnSpinResultDone);
		//This closes the screen before anything can be selected, so it is being removed
		//OnSetReturn(expectedReturnModifier);
		onUpdateBet.RaiseEvent(bet);
	}

	void Update()
	{
		spinTimeLeft -= Time.deltaTime;
		if(spinTimeLeft <= 0)
		{
			if (state == 0)
			{
				state = 1;
				currentWheel = 0;
				spinTimeLeft = Random.value * 1 + 0.5f;
			}
			else if (state == 1)
			{
				onAudioEvent.OnPlayEvent(stopSpinAudioClip);

				wheels[currentWheel].spinning = false;
				wheels[currentWheel].fixing = true;
				if(currentWheel != 2)
				{
					currentWheel++;
					spinTimeLeft = Random.value * (0.5f / currentWheel) + 0.5f;
				}
				else
				{
					state = 2;
					reward = CalculateRewards(new int[] { wheels[0].Result, wheels[1].Result, wheels[2].Result });
					spinTimeLeft = 0.5f;
				}
			}
			else if (state == 2)
			{
				winnings = (int)(reward * bet * (currentSpin == 19 ? 2 * expectedReturnModifier : expectedReturnModifier));
				winningsDisplay.text = winnings.ToString("000");
				//winningsDisplay.text = (reward * int.Parse(betDisplay.text) *
				//	(currentSpin == 19 ? 2 * expectedReturnModifier : expectedReturnModifier)).ToString();
				//state = 3;
				//spinTimeLeft = 2.0f;

				onSpinResult.RaiseEvent(winnings);
				state = -1; // wait in this state until spin result done
			}
			else if (state == 3)
			{
				credits += winnings;
				winnings = 0;
				if (bet > credits)
				{
					bet = (credits / 10) * 10;
					onUpdateBet.RaiseEvent(bet);
				}
				creditsDisplay.text = credits.ToString("000");
				betDisplay.text = bet.ToString("000");
				winningsDisplay.text = winnings.ToString("000");
				state = 4;
			}
			else if (state == -2)
			{
                // initialize wheels
                state = 4;
                wheels[0].spinning = false;
                wheels[1].spinning = false;
                wheels[2].spinning = false;
                wheels[0].fixing = true;
                wheels[1].fixing = true;
                wheels[2].fixing = true;
            }
		}
	}

	void OnSpinResultDone()
	{
		state = 3;
	}

	#region Rewards

	public float CalculateRewards(int[] results)
	{
		List<float> potentialResults = new List<float>();
		if (results[0] == results[1])
		{
			if (results[1] == results[2])
			{
				potentialResults.Add(GetTripleReward(results[0]));
			}
			else
			{
				potentialResults.Add(GetDoubleReward(results[0]));
			}
		}
		else if (results[1] == results[2] || results[0] == results[2])
		{
			potentialResults.Add(GetDoubleReward(results[1]));
		}
		potentialResults.Add(GetReward(results[0]));
		potentialResults.Add(GetReward(results[1]));
		potentialResults.Add(GetReward(results[2]));
		potentialResults.Sort();
		return potentialResults[potentialResults.Count - 1];
	}

	private float GetTripleReward(int tile)
	{
		switch (tile)
		{
			case 1:
				return 4;  //A
			case 2:
				return 6;  //A
			case 3:
				return 8;  //A
			case 4:
				return 10;  //A
			case 5:
				return 12; //A
			case 6:
				return 18; //B
			case 7:
				return 24; //B
			case 8:
				return 30; //B
			case 9:
				return 36; //B
			case 10:
				return 42; //B
			case 11:
				return 60; //C
			case 12:
				return 78; //C
			case 13:
				return 96; //C
			case 14:
				return 114; //C
			case 15:
				return 168; //D
			case 16:
				return 222;//D
			case 17:
				return 276;//D
			case 18:
				return 438;//E
			case 19:
				return 1000;

			default:
				return 2;
		}
	}

	private float GetDoubleReward(int tile)
	{
		switch (tile)
		{
			case 1:
				return 0.4f;  //A
			case 2:
				return 0.6f;  //A
			case 3:
				return 0.8f;  //A
			case 4:
				return 1.0f;  //A
			case 5:
				return 1.2f; //A
			case 6:
				return 1.8f; //B
			case 7:
				return 2.4f; //B
			case 8:
				return 3; //B
			case 9:
				return 3.6f; //B
			case 10:
				return 4.2f; //B
			case 11:
				return 6.0f; //C
			case 12:
				return 7.8f; //C
			case 13:
				return 9.6f; //C
			case 14:
				return 11.4f; //C
			case 15:
				return 16.8f; //D
			case 16:
				return 22.2f;//D
			case 17:
				return 27.6f;//D
			case 18:
				return 43.8f;//E
			case 19:
				return 100;

			default:
				return 0.2f;
		}
	}

	private float GetReward(int tile)
	{
		switch (tile)
		{
			case 10:
			case 11:
			case 12:
				return 0.2f; //C
			case 13:
			case 14:
				return 0.4f; //C
			case 15:
				return 0.8f; //D
			case 16:
				return 1;//D
			case 17:
				return 1.2f;//D
			case 18:
				return 2;//E
			case 19:
				return 5;

			default:
				return 0;
		}
	}
	#endregion // Rewards

	#region Buttons

	public void OnStartSpin()
	{
		if (state == 4 && bet > 0 && bet <= credits)
		{
			onAudioEvent.OnPlayEvent(startSpinAudioClip);

			wheels[0].spinning = true;
			wheels[1].spinning = true;
			wheels[2].spinning = true;
			currentSpin++;
			if (currentSpin == 20)
			{
				currentSpin = 0;
			}
			state = 0;
			spinTimeLeft = 1 + Random.value * 2;

			credits -= bet;
			creditsDisplay.text = credits.ToString("000");
			onSpinStart.RaiseEvent();
		}
	}

	public void IncreaseBet()
	{
		if (state == 4)
		{
			int addBet = ((expectedReturnModifier == 0.5f || expectedReturnModifier == 1.5f) ? 10 : 5);
			if (bet + addBet > credits) return;

			bet += addBet;
			onUpdateBet.RaiseEvent(bet);
			betDisplay.text = bet.ToString("000");
			creditsDisplay.text = credits.ToString("000");

			onAudioEvent.OnPlayEvent(betupAudioClip);
		}
	}
	public void DecreaseBet()
	{
		if (state == 4)
		{
			int subtractBet = ((expectedReturnModifier == 0.5f || expectedReturnModifier == 1.5f) ? 10 : 5);
			if (bet <= 0) return;

			bet -= subtractBet;
			onUpdateBet.RaiseEvent(bet);
			betDisplay.text = bet.ToString("000");
			creditsDisplay.text = credits.ToString("000");
			onAudioEvent.OnPlayEvent(betdownAudioClip);
		}
	}

	public void OnApplyCredits(int credit)
	{
		// check if we are starting the game
		if (credits == 0)
		{
			onStartGame.RaiseEvent();
		}

		onAudioEvent.OnPlayEvent(creditsAudioClip);

		credits += credit;
		//Cap caused issues when pressed after receiving a reward, so removed to reduce risk of accidental clearing of winnings
		//if (credits > 100) credits = 100;
		creditsDisplay.text = credits.ToString("000");
	}

	public void OnCashOut()
	{
		onStopGame.RaiseEvent();
		if (credits > 0) onAudioEvent.OnPlayEvent(cashoutAudioClip);

		credits = 0;
		bet = 0;
		onUpdateBet.RaiseEvent(bet);
		creditsDisplay.text = credits.ToString("000");
		betDisplay.text = bet.ToString("000");

	}

	#endregion // Buttons

	#region Returns

	public void OnStartReturnScreen()
	{
		startMenu.SetActive(true);
	}

	public void OnSetReturn(float expectedReturn)
	{
		expectedReturnModifier = expectedReturn;
		startMenu.SetActive(false);
	}

	#endregion // RETURNS
}
