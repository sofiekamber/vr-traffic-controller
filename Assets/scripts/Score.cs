using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static int counter = 0;

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshPro>().text = counter.ToString();
    }
}
