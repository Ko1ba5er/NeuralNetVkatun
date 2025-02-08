using UnityEngine;

public class Cat : MonoBehaviour
{
    public const float speed = 10;
    public const float rotateSpeed = 60;
    public NeuralNetwork Brain = new NeuralNetwork(6, 3);
    public int ate = 0;

    public bool dead = false;

    private void FixedUpdate()
    {
        if (dead)
            return;

        Debug.DrawRay(transform.position, (transform.forward * 2 - transform.right).normalized * 20);
        Debug.DrawRay(transform.position, transform.forward);
        Debug.DrawRay(transform.position, (transform.forward * 2 + transform.right).normalized * 20);
        float[] inputs = new float[6];
        for (int i = 0; i < 3; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward * 2 + transform.right * (i - 1)), out hit, 40))
            {
                // 0 - wall, 0.2 - Player, 0.7 - Empty, 1 - food
                // distance / maxDistance
                if (hit.collider.tag == "Wall")
                    inputs[i * 2] = 0;
                if (hit.collider.tag == "Player")
                    inputs[i * 2] = 0.2f;
                if (hit.collider.tag == "Food")
                    inputs[i * 2] = 1;

                inputs[i * 2 + 1] = hit.distance / 40;
            }
            else
            {
                inputs[i * 2] = 0.7f;
                inputs[i * 2 +1] = 40 / 40;
            }
        }

        float[] results = Brain.proccess(inputs);

        if (results[0] == 1f)
            transform.position += speed * transform.forward * Time.fixedDeltaTime;

        if (results[1] == 1f)
            transform.Rotate(transform.up, rotateSpeed * Time.fixedDeltaTime);
        else
            if (results[2] == 1f)
                transform.Rotate(transform.up, - rotateSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            ate++;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Wall")
        {
            ate--;
            dead = true;
        }
    }
}