using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    public float Speed = 10f;
    public AudioClip walkingSound;
    AudioSource _audioSource;

    private Vector3 _input;
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Camera _camera;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        UpdateAnimation();
        RotateToCameraDirection();

    }

    void RotateToCameraDirection()
    {
        Vector3 newForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);
        newForward.Normalize();

        if (newForward != Vector3.zero)
        {
            transform.forward = newForward;
        }
        else
        {
            transform.forward = _camera.transform.up;
        }
    }

    void UpdateAnimation()
    {
        if (_animator == null)
        {
            return;
        }

        bool isWalking = _input.magnitude > 0.01f;

        if (isWalking && !_animator.GetBool("IsWalking"))
        {
            _animator.SetBool("IsWalking", true);
            _audioSource.clip = walkingSound;
            _audioSource.Play();

        }

        else if (!isWalking && _animator.GetBool("IsWalking"))
        {
            _animator.SetBool("IsWalking", false);
            _audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = _input;

        movement = _camera.transform.TransformDirection(movement);
        movement = Vector3.ProjectOnPlane(movement, Vector3.up).normalized;
        movement *= Speed;

        if(_input != Vector3.zero && movement == Vector3.zero)
        {
            movement = transform.TransformDirection(_input).normalized * Speed;
        }

        _rigidbody.velocity = new Vector3(movement.x, 0, movement.z);
    }
}
