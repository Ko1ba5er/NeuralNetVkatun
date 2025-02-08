public class Neuron
{
    public float proccess(float input)
    {
        if (input > 0.5f)
            return 1;

        return 0;
    }
}