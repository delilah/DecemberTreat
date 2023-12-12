using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateShrine : MonoBehaviour
{
    [SerializeField] private GameObject _sphere;
    [SerializeField] private float speed = 0.8f;
    [SerializeField] private float amplitude = .5f;
    [SerializeField] private float offsetY = 1.2f; 
    [SerializeField] private Material goldMaterial;
    private float time;
    private float platformY;
     private bool startMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            startMoving = true;
            platformY = this.transform.position.y;
            _sphere.transform.position = new Vector3(_sphere.transform.position.x, platformY - offsetY, _sphere.transform.position.z);
            GetComponent<Renderer>().material = goldMaterial;
        
        }
    }

    private void Update()
    {
        if (startMoving)
        {

            // Start moving up and down
                float newY = platformY + offsetY + amplitude * Mathf.Sin(time * speed);
                _sphere.transform.position = new Vector3(_sphere.transform.position.x, newY, _sphere.transform.position.z);
                time += Time.deltaTime;

            // Move the object to the targetY
            if (_sphere.transform.position.y != platformY + offsetY)
            {
                float step = speed * Time.deltaTime;
                Vector3 targetPosition = new Vector3(_sphere.transform.position.x, platformY + offsetY, _sphere.transform.position.z);
                _sphere.transform.position = Vector3.MoveTowards(_sphere.transform.position, targetPosition, step);
            }
        }
    }
}