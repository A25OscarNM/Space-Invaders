using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{
    [SerializeField] private float force = 5f; // Fuerza del movimiento
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float duration;
    [SerializeField] int blinkNum;
    [SerializeField] GameObject shootPrefab;
    [SerializeField] float shootOffset = 0.5f;

    [SerializeField] GameObject explosion;
    Vector3 initialPosition;

    [SerializeField] GameManager gameManager;

    bool active = false;
    private Rigidbody2D rb; // Referencia al componente Rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("StartPlayer");
        initialPosition = transform.position;
    }

    void Update()
    {
        if (active && Input.GetKeyDown(KeyCode.Space))
        {
            // Calcular la posición donde se creará el disparo (un poco por delante de la nave)
            Vector3 shootPosition = transform.position + Vector3.up * shootOffset;

            // Crear el disparo en la posición calculada y sin rotación
            Instantiate(shootPrefab, shootPosition, Quaternion.identity);
        }
    }

    private void FixedUpdate()
    {
        if (active)
            CheckMove();
    }

    private void CheckMove()
    {
        // Obtenemos la dirección del movimiento en los ejes horizontal y vertical
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        direction.Normalize(); // Normalizamos el vector para que tenga magnitud 1

        // Aplicamos una fuerza en la dirección obtenida
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    IEnumerator StartPlayer()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        Color color = mat.color;
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        Vector3 initialPosition = transform.position;
        float t = 0, t2 = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(initialPosition, endPosition, t / duration);
            transform.position = newPosition;

            t2 += Time.deltaTime;
            float newAlpha = blinkNum * (t2 / duration);
            if (newAlpha > 1)
            {
                t2 = 0;
            }
            color.a = newAlpha;
            mat.color = color;
            yield return null;
        }

        color.a = 1;
        mat.color = color;
        collider.enabled = true;
        active = true;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Enemy" || tag == "asteroid")
        {
            DestroyShip();
            gameManager.lifeDown();
        }
    }

    void DestroyShip()
    {
        gameManager.lifeDown();

        // Desactivar comportamiento
        active = false;
        // Instanciar la animación de la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);
        // Resetear posición de la nave
        transform.position = initialPosition;
        // Reiniciar la nave
        StartCoroutine("StartPlayer");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Colisión con disparo");
    }
}