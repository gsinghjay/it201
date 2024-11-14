using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private readonly Vector3 rotationSpeed = new Vector3(15f, 30f, 45f);
    private Transform cachedTransform;

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void Update()
    {
        cachedTransform.Rotate(rotationSpeed * Time.deltaTime);
    }
}