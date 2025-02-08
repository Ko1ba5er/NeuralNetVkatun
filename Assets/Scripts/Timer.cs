using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = ((int)Time.time / 60) + ":" + ((int)Time.time % 60).ToString("00");
    }
}
