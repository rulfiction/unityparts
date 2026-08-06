using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    [Header("Pickup")]
    public string playerTag = "Player";
    public bool destroyOnPickup = true;

    [Header("Rotation")]
    public float rotationSpeed = 90f;

    [Header("Floating")]
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;

        Collider itemCollider = GetComponent<Collider>();
        itemCollider.isTrigger = true;
    }

    private void Update()
    {
        RotateItem();
        FloatItem();
    }

    private void RotateItem()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void FloatItem()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        PickUp();
    }

    private void PickUp()
    {
        Debug.Log("Предмет подобран: " + gameObject.name);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
