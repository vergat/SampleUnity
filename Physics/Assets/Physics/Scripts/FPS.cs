using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    // Serializable fields

    [SerializeField]
    private Text m_FPS = null;

    // Fields

    float deltaTime = 0.0f;

    // MonoBehaviour's interface

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        if (m_FPS != null)
        {
            m_FPS.text = text;
        }
    }
}
