using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private Wheel[] wheels;
    private int state = 3;
    private float spinTimeLeft = 0f;
    private int currentWheel = 0;
    public float currentSpin = 0;
    public float expectedReturnModifier = 1;
    [SerializeField] private Canvas betCanvas;
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private Canvas jackpotDisplay;
    [SerializeField] private TMP_Text jackpotText;
    [SerializeField] private Canvas winDisplay;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Canvas loseDisplay;
    [SerializeField] private Canvas startMenu;

    // Update is called once per frame
    void Update()
    {
        spinTimeLeft -= Time.deltaTime;
        if(spinTimeLeft <= 0)
        {
            if (state == 0)
            {
                if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.anyKeyDown)
                {
                    state = 1;
                    currentWheel = 0;
                    spinTimeLeft = Random.value * 1 + 0.5f;
                }
            }
            else if (state == 1)
            {
                wheels[currentWheel].spinning = false;
                if(currentWheel != 2)
                {
                    currentWheel++;
                    spinTimeLeft = Random.value * (0.5f / currentWheel) + 0.5f;
                }
                else
                {
                    state = 2;
                    spinTimeLeft = 1.5f;
                }
            }
            else if(state == 2)
            {
                float reward = CalculateRewards(new int[] { wheels[0].Result, wheels[1].Result, wheels[2].Result});
                if(reward == 1000)
                {
                    jackpotDisplay.enabled = true;
                    jackpotText.text = $"{(currentSpin == 19 ? "BONUS " : "")}JACKPOT! YOU WIN {int.Parse(betInput.text) * reward * expectedReturnModifier *(currentSpin == 19 ? 2 : 1)} TOKENS!";
                }
                else if(reward > 0)
                {
                    winDisplay.enabled = true;
                    winText.text = $"Congratulations, you {(currentSpin == 19 ? "got the bonus and " : "")}win {int.Parse(betInput.text) * reward * expectedReturnModifier * (currentSpin == 19 ? 2 : 1)} tokens.";
                }
                else
                {
                    loseDisplay.enabled = true;
                }
                state = 3;
            }
        }
    }

    public void CloseResult()
    {
        winDisplay.enabled = false;
        loseDisplay.enabled = false;
        jackpotDisplay.enabled = false;
        betCanvas.enabled = true;
    }

    public void InitiateSpin()
    {
        wheels[0].spinning = true;
        wheels[1].spinning = true;
        wheels[2].spinning = true;
        betCanvas.enabled = false;
        currentSpin++;
        if(currentSpin == 20)
        {
            currentSpin = 0;
        }
        state = 0;
    }

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

    public void Initialize(float expectedReturn)
    {
        expectedReturnModifier = expectedReturn;
        startMenu.enabled = false;
    }
}
