public class HeavisideStepNeuron : INeuron
{
    public float proccess(float input)
    {
        if (input < 0)
            return 0;

        return 1;
    }
}