using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Ehyal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            float flip = (horizontal > 0) ? 0.5f : -0.5f;
            transform.localScale = new Vector3(flip, 0.5f, 0.5f);
        }
    }
}
