using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreGet : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        int finalScore = ScoreStore.score;
        if(finalScore < 0) { finalScore = 0; }
        if(finalScore > 9999) { finalScore = 9999; }
        text.text = "Final Score: " + AddedZeros(finalScore) + finalScore;
    }
    string AddedZeros(int number)
    {
        string result = "";
        //adds zeroes to make a 4 digit number
        if(number < 1000) { result += "0"; }
        if (number < 100) { result += "0"; }
        if (number < 10) { result += "0"; }
        return result;
    }
}
