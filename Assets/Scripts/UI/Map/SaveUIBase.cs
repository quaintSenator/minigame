using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUIBase : MonoBehaviour
{
    public virtual void OnBgMaskClick()
    {
        gameObject.SetActive(false);
    }
}
