using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField] private float speed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
        _controller.Move(move * (Time.deltaTime * speed));
    }
}
