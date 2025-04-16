using UnityEngine;
using UnityEngine.UI;

public class Dino : NNAgent
{
    private float[] results;
    private bool onGround = false;

    private Rigidbody2D rb;

    public override Color color
    {
        get => GetComponent<Image>().color;
        set => GetComponent<Image>().color = value;
    }

    private void Awake()
    {
        Brain = new NeuralNetwork(3, 2);
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        float[] inputs = new float[] { DinoRoom.NearestObstacleX() / 1000, DinoRoom.speed / 1200, DinoRoom.NearestObstacleType() };
        results = Brain.proccess(inputs);

        if (results[0] >= 0f)
            jump();

        if (results[1] >= 0f && onGround)
            transform.localScale = new Vector3(1f, 0.5f, 1f);
        else
            transform.localScale = Vector3.one;
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.Space))
    //        jump();

    //    if (Input.GetKey(KeyCode.S) && onGround)
    //        transform.localScale = new Vector3(1f, 0.5f, 1f);
    //    else
    //        transform.localScale = Vector3.one;
    //}

    private float Lastjumptime = 0;

    private void jump()
    {
        if (onGround && Time.time - Lastjumptime >= 0.5f)
        {
            rb.AddForce(Vector3.up * 20000);
            Lastjumptime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            dead = true;
            transform.position += Vector3.up * 700;
            rb.simulated = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food")
            onGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Food")
            onGround = false;
    }

    public override void Revive(NeuralNetwork brain, Color color)
    {
        dead = false;
        transform.position = Vector3.right * 100 + Vector3.up * 100;
        rb.simulated = true;
        score = 0;
        GetComponent<Image>().color = color;
        Brain = brain;
    }
}