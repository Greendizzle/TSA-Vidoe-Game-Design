using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kill : MonoBehaviour
{
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Application.LoadLevel("Lock");
    }
}
