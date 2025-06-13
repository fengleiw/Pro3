using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pillarQuake : MonoBehaviour
{
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        if (anim != null && anim.clip != null)
        {
            // Destroy after animation length
            Destroy(gameObject, anim.clip.length);
        }
        else
        {
            // Fallback if no animation found
            Destroy(gameObject, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
