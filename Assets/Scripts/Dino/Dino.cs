using UnityEngine;

public class Dino : MonoBehaviour
{
    public const float speed = 10;
    public const float rotateSpeed = 60;
    [SerializeField]
    public NeuralNetwork Brain = new NeuralNetwork(1, 1);
    [SerializeField]
    private float[] results;
    public int score = 0;

    public bool dead = false;
    private bool onGround = false;

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        //inputs[0] = 0;//;transform.position.y / 200;
        float[] inputs = new float[] { DinoRoom.NearestObstacleX() / 1000 };
        results = Brain.proccess(inputs);
        //inputs[2] = 0;

        if (results[0] >= 0f)
            jump();

        //if (results[1] == 1f)
        //    crawl();
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.Space))
    //        jump();
    //}

    private float Lastjumptime = 0;

    private void jump()
    {
        if (onGround && Time.time - Lastjumptime >= 0.5f)
        {
            rb.AddForce(Vector3.up * 20000);
            Lastjumptime = Time.time;
            score -= 1;
        }
    }

    private void crawl()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            dead = true;
            transform.position += Vector3.up * 700;
            rb.simulated = false;
        }

        if (collision.gameObject.tag == "Food")
            onGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Food")
            onGround = false;
    }
}