public class LinearNeuron : INeuron
{
    public float proccess(float input)
    {
        if (input <= -1)
            return -1;
        if (input >= 1)
            return 1;

        return input;
    }
}