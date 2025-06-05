using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public GameManager gameManager;

    private Rigidbody2D rb;
    private Animator anim;
    private int saltosRealizados = 0;
    private int maxSaltos = 2;

    private float fuerzaSaltoPrimario = 6f;
    private float fuerzaMiniSalto = 4.5f;

    private float gravedadExtra = 2.5f;

    private float velocidadInicialSalto = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (gameManager.start && saltosRealizados < maxSaltos)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("estaSaltando", true);

                // Se decide si es el primer salto o primer y segundo salto
                float fuerzaAplicar = (saltosRealizados == 0) ? fuerzaSaltoPrimario : fuerzaMiniSalto;
                rb.AddForce(new Vector2(0, fuerzaAplicar), ForceMode2D.Impulse);

                // Se guarda la velocidad vertical inicial para calcular gravedad extra después
                velocidadInicialSalto = rb.velocity.y;

                saltosRealizados++;
            }
        }


        if (rb.velocity.y < 0)
        {
            // Se aplica gravedad adicional para hacer la caída más rápida
            if (velocidadInicialSalto > 0 && Mathf.Abs(rb.velocity.y) > 0)
            {
                gravedadExtra = velocidadInicialSalto / Mathf.Abs(rb.velocity.y);
                if (gravedadExtra < 1f) gravedadExtra = 1f;

                rb.velocity += Vector2.up * Physics2D.gravity.y * (gravedadExtra - 1) * Time.deltaTime;
            }
        }

        // Si se acaba el juego, se destruye el jugador
        if (gameManager.gameOver)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            // Al tocar el suelo, se reinician los saltos y la animación
            anim.SetBool("estaSaltando", false);
            saltosRealizados = 0;
            velocidadInicialSalto = 0f;
            gravedadExtra = 2.5f;
        }

        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            // Si colisiona con un obstáculo, se activa el game over
            gameManager.gameOver = true;
        }
    }
}
