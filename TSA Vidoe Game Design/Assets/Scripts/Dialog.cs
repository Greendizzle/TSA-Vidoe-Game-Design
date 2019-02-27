﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialog : MonoBehaviour {

    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public GameObject continueButton;
    public int nextscene;
    private int buttonTimes;
    public int numberofsentences;


    void Update()
    {
        if (textDisplay.text == sentences[index]) {
            continueButton.SetActive(true);

        }


    }


    void Start()
    {
        StartCoroutine(Type());
    }

    IEnumerator Type() {

        foreach (char letter in sentences[index].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);

        }


    }

    public void NextSentence() {

        continueButton.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        else { textDisplay.text = ""; }
        continueButton.SetActive(false);
        Debug.Log("It worked?");
        nextscene += 1;


    }

    public void buttonPress() {


        buttonTimes += 1;

}
    

}
