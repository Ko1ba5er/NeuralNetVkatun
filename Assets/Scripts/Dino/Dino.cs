using UnityEngine;

public class Dino : MonoBehaviour
{
    [SerializeField]
    public NeuralNetwork Brain = new NeuralNetwork(3, 2);
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
}