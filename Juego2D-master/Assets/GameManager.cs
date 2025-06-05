using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float velocidad = 2f;
    public float incrementoVelocidad = 0.1f;
    public float tiempoGeneracion = 3.0f;

    public Renderer bg; // Fondo para simular movimiento
    public GameObject col1; // Prefab del suelo
    public GameObject piedra1; // Prefab de obstáculo tipo 1
    public GameObject piedra2; // Prefab de obstáculo tipo 2

    public bool start = false;
    public bool gameOver = false;

    public GameObject menuInicio;
    public GameObject menuGameOver;

    public List<GameObject> suelo = new List<GameObject>();
    public List<GameObject> obstaculos = new List<GameObject>();

    public float separacionMinima = 3f; // Separación mínima entre obstáculos para evitar que se junten demasiado

    private void Start()
    {

        for (int i = 0; i < 21; i++)
        {
            suelo.Add(Instantiate(col1, new Vector2(-10 + i, -3), Quaternion.identity));
        }

        // Se instancian dos obstáculos iniciales
        obstaculos.Add(Instantiate(piedra1, new Vector2(15, -2), Quaternion.identity));
        obstaculos.Add(Instantiate(piedra2, new Vector2(25, -2), Quaternion.identity));

        StartCoroutine(GenerarObstaculos());
        StartCoroutine(AumentarVelocidad());
    }

    private void Update()
    {
        if (!start && !gameOver)
        {
            // Menú de inicio activo hasta que se presione X
            menuInicio.SetActive(true);
            if (Input.GetKeyDown(KeyCode.X))
            {
                start = true;
            }
        }
        else if (gameOver)
        {
            menuGameOver.SetActive(true);
            if (Input.GetKeyDown(KeyCode.X))
            {
                // Reinicia la escena actual
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {

            menuInicio.SetActive(false);
            menuGameOver.SetActive(false);

            // Movimiento del fondo para simular desplazamiento
            bg.material.mainTextureOffset += new Vector2(0.015f, 0) * velocidad * Time.deltaTime;

            // Mueve bloques de suelo hacia la izquierda
            foreach (GameObject bloque in suelo)
            {
                if (bloque.transform.position.x <= -10)
                {
                    bloque.transform.position = new Vector3(10f, -3, 0);
                }
                bloque.transform.position += new Vector3(-1, 0, 0) * velocidad * Time.deltaTime;
            }


            foreach (GameObject obs in obstaculos)
            {
                if (obs.transform.position.x <= -10)
                {
                    float nuevaX = GenerarPosicionXConSeparacion();
                    obs.transform.position = new Vector3(nuevaX, -2, 0);
                }
                obs.transform.position += new Vector3(-1, 0, 0) * velocidad * Time.deltaTime;
            }
        }
    }

    IEnumerator GenerarObstaculos()
    {
        while (!gameOver)
        {
            if (start)
            {
                float posX = GenerarPosicionXConSeparacion();
                GameObject nuevo = Instantiate(Random.value > 0.5f ? piedra1 : piedra2, new Vector3(posX, -2, 0), Quaternion.identity);
                obstaculos.Add(nuevo);
            }

            float espera = Random.Range(tiempoGeneracion, tiempoGeneracion + 0.8f);
            yield return new WaitForSeconds(espera);
        }
    }

    IEnumerator AumentarVelocidad()
    {
        while (!gameOver)
        {
            if (start)
            {
                yield return new WaitForSeconds(2f);
                velocidad += incrementoVelocidad;
            }
            else
            {
                yield return null;
            }
        }
    }

    // Genera una posición X aleatoria asegurando separación mínima entre obstáculos
    private float GenerarPosicionXConSeparacion()
    {
        float posX;
        int intentos = 0;
        do
        {
            posX = Random.Range(18, 25);
            intentos++;
        } while (!PosicionValida(posX) && intentos < 20);

        return posX;
    }

    // Verifica que la posición generada no esté demasiado cerca de otros obstáculos
    private bool PosicionValida(float posX)
    {
        foreach (GameObject obs in obstaculos)
        {
            if (Mathf.Abs(obs.transform.position.x - posX) < separacionMinima)
            {
                return false;
            }
        }
        return true;
    }
}
