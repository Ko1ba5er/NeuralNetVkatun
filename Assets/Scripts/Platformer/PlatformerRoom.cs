using UnityEngine;

public class PlatformerRoom : Room
{
    public float timer;

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
}