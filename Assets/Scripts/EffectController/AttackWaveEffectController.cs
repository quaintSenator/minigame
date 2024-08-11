using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWaveEffectController : MonoBehaviour
{
    [SerializeField] public Material m_mat;
    
    // Start is called before the first frame update
    void Start()
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer)
        {
            m_mat = meshRenderer.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (m_mat)
            {
                m_mat.SetFloat("_TimeOfStart", Time.time);
            }
            else
            {
                Debug.Log("m_mat = null");
            }
        }
    }
}
