using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    #region Variables
    //Components
    private Animator anim;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    #endregion

    #region Private Methods
    public void Deselected()
    {
        anim.SetTrigger("Deselected");
    }

    public void Selected()
    {
        anim.SetTrigger("Selected");
    }

    public void Pressed()
    {
        anim.SetTrigger("Pressed");        
    }
    #endregion
}