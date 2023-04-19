using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    [SerializeField]
    public GameObject target;
    [SerializeField]
    public Vector3 startPos;
    [SerializeField]
    public Vector3 currentPos;
    [SerializeField]
    public Vector3 targetPos;
    [SerializeField]
    public bool shouldMove;
    [SerializeField]
    public bool moveSelected;
    [SerializeField]
    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = this.gameObject.transform.position;

        if (moveSelected == true)
        {
            MoveToTarget();
        }
        if (moveSelected == false)
        {
            shouldMove = false;
            anim.ResetTrigger("Moving");
        }
        if (shouldMove == true)
        {
            Vector3 targetAdjust = new Vector3(targetPos.x, currentPos.y, targetPos.z);
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, targetAdjust, 10 * Time.deltaTime);

        }
    }

    public void MoveToTarget()
    {
        shouldMove = true;
        startPos = this.gameObject.transform.position;
        targetPos = target.transform.position;
        Vector3 relativePos = this.gameObject.transform.position - target.transform.position;
        Vector3 adjustedPos = new Vector3(relativePos.x, 0, relativePos.z);
        Quaternion rotation = Quaternion.LookRotation(adjustedPos, Vector3.up);
        this.gameObject.transform.rotation = rotation;
        anim.SetTrigger("Moving");
    }
}
