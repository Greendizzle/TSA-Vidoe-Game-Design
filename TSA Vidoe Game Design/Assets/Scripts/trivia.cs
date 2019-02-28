using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class trivia : MonoBehaviour
{
    private int score;


   
    
    public void RightAnswer()
    {
        score = score + 1;
    }



    public 


    // Update is called once per frame
    void Update()
    {

        Debug.Log(score);



        if (score == 4) {
            Debug.Log("You made it");

            Application.LoadLevel("AfterTrivia");

        }
    }
}
