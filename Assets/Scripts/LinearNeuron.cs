public class LinearNeuron : INeuron
{
    public float proccess(float input)
    {
        if (input <= 0)
            return 0;
        if (input >= 1)
            return 1;

        return input;
    }
}