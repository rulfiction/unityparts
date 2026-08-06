using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // Объявление переменных
    public int speed = 20;
    public int turnSpeed = 50;
    public InputAction moveAction;
    public Vector2 moveInput;
    void Start()
    {
        // включение бинда
        moveAction.Enable();
}
    
    void Update() // выполняется каждый кадр
    {
        //передача значений
        moveInput = moveAction.ReadValue<Vector2>();



        transform.Translate(Vector3.forward * Time.deltaTime * speed * moveInput.y); 
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * moveInput.x); 
        // Vector3.right left up down back
        // (0, 0, 1) (1, 0, 0) (-1, 0, 0) (0, 1, 0) (0, -1, 0)
    }
}
