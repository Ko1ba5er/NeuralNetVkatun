using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void FixedUpdate()
    {
        text.text = (int)Time.time / 3600 + ":" + ((int)Time.time / 60 % 60).ToString("00") + ":" + ((int)Time.time % 60).ToString("00");
    }
}