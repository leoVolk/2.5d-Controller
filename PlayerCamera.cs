using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed;
    public Vector3 Offset;

    // Start is called before the first frame update
    void Start()
    {
        if(Target == null){
            Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(Target.position.x, Target.position.y, 10));
        transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, transform.position.y, transform.position.z) + Offset, Time.deltaTime * SmoothSpeed);
    }
}
