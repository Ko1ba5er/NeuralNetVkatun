using UnityEngine;
using UnityEngine.UI;

public class FlappyDino : NNAgent
{
    private Rigidbody2D rb;
    private float[] results;

    public override Color color
    {
        get => GetComponent<Image>().color;
        set => GetComponent<Image>().color = value;
    }

    private void Awake()
    {
        Brain = new NeuralNetwork(4, 1);
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        float[] inputs = new float[]
        {
            FlappyDinoRoom.NearestObstacleX() / 1000,
            FlappyDinoRoom.NearestObstacleY() / ((RectTransform)transform.parent).rect.size.y,
            rb.linearVelocityY / 500,
            transform.position.y / ((RectTransform)transform.parent).rect.size.y
        };

        results = Brain.proccess(inputs);

        if (results[0] >= 0f)
            rb.linearVelocity = Vector2.up * 400;
    }

    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rb.linearVelocityY / 500 * 75), 5);

        //if (Input.GetKeyDown(KeyCode.Space))
        //    rb.linearVelocity = Vector2.up * 400;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            dead = true;
            transform.position += Vector3.up * (((RectTransform)transform.parent).rect.size.y);
            rb.simulated = false;
        }
    }

    public override void Revive(NeuralNetwork brain, Color color)
    {
        dead = false;
        transform.position = Vector3.right * (((RectTransform)transform.parent).rect.size.x / 5) + Vector3.up * (((RectTransform)transform.parent).rect.size.y / 2);
        rb.simulated = true;
        score = 0;
        GetComponent<Image>().color = color;
        Brain = brain;
    }
}