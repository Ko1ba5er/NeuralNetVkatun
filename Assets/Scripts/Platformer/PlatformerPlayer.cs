using UnityEngine;

public class PlatformerPlayer : NNAgent
{
    private float[] results;
    private float[] inputs = new float[18];
    private bool onGround = false;

    private Rigidbody2D rb;

    private bool _dead = false;
    public override bool dead
    {
        get { return _dead; }
        set
        {
            _dead = value;
            if (_dead)
                score += 15 - (GameObject.Find("Exit").transform.position.x - transform.position.x);
        }
    }

    public override Color color
    {
        get => GetComponent<SpriteRenderer>().color;
        set => GetComponent<SpriteRenderer>().color = value;
    }

    private void Awake()
    {
        Brain = new NeuralNetwork(18, 3);
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        for (int i = 0; i < 9; i++)
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.right * 4 + transform.up * (i - 4), Color.red);

        for (int i = 0; i < 9; i++)
        {
            //Дорогой дневник, мне не описать всю туц боль и унижения, которые я испытал сегодня, когда Ботриибмак обозвал меня клоуном на стриме прилюдно(((((((((((
            RaycastHit2D hit;
            if (hit = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, transform.right * 4 + transform.up * (i - 4), 100, LayerMask.GetMask("Default")))
            {
                if (hit.transform.name == "Exit")
                    inputs[i * 2] = 1;
                else if (hit.transform.tag == "Wall")
                    inputs[i * 2] = 0;
                else
                    inputs[i * 2] = 0.5f;

                inputs[i * 2 + 1] = hit.distance / 5;
            }
            else
            {
                inputs[i * 2] = 0.5f;
                inputs[i * 2 + 1] = 100;
            }

        }

        results = Brain.proccess(inputs);

        if (results[0] >= 0f)
            jump();

        if (results[1] >= 0f)
            transform.position += Vector3.right * 4 * Time.deltaTime;
        if (results[2] >= 0f)
            transform.position -= Vector3.right * 4 * Time.deltaTime;
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.Space))
    //        jump();

    //    if (Input.GetKey(KeyCode.D))
    //        transform.position += Vector3.right * 4 * Time.deltaTime;
    //    if (Input.GetKey(KeyCode.A))
    //        transform.position -= Vector3.right * 4 * Time.deltaTime;
    //}

    private float Lastjumptime = 0;

    private void jump()
    {
        if (onGround && Time.time - Lastjumptime >= 0.5f)
        {
            rb.AddForce(Vector3.up * 300);
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
            //score -= 20f;
        }

        if (collision.gameObject.name == "Exit")
        {
            dead = true;
            transform.position += Vector3.right * 1f;
            rb.simulated = false;
            score += 1000f;
            FindFirstObjectByType<PlatformerRoom>().finished++;
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
        base.Revive(brain, color);

        transform.position = Vector3.right * -7.04f + Vector3.up * 1.78f;
        dead = false;
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        score = 0;
        GetComponent<SpriteRenderer>().color = color;
    }
}