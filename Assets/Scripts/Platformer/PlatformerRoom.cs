using UnityEngine;

public class PlatformerRoom : Room
{
    public float timer;

    private int _finished;
    public int finished { get => _finished; set { _finished = value; finishedAmount.text = "Кончившие: " + _finished.ToString(); } }

    [SerializeField] private TMPro.TMP_Text finishedAmount;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        timer += Time.fixedDeltaTime;

        if (timer > 20f)
        {
            timer = 0;
            Restart();
        }
    }

    protected override void Restart()
    {
        finished = 0;

        base.Restart();
    }
}