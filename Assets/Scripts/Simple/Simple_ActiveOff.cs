using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Simple_ActiveOff : MonoBehaviour
{
    public bool Time_Check = false;
    public float Timer = 0f;
    public GameObject Target;
    public UnityEvent Active_Func = new UnityEvent();

    Coroutine co;
    UnityEvent next_func = new UnityEvent();
    public void Set(UnityAction event_fun = null)
    {
        this.gameObject.SetActive(true);

        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }


        next_func.RemoveAllListeners();
        if (event_fun != null)
            next_func.AddListener(event_fun);

        co = StartCoroutine(Active_co());
    }

    IEnumerator Active_co()
    {
        if (Time_Check)
        {
            yield return new WaitForSeconds(Timer);
        }
        else
        {

        }

        if (Target != null)
            Target.SetActive(false);
        else
            this.gameObject.SetActive(false);

        if (next_func != null)
            next_func.Invoke();

        if (Active_Func != null)
            Active_Func.Invoke();
    }

    public void Active_Off()
    {
        this.gameObject.SetActive(false);

        if (next_func != null)
            next_func.Invoke();

        if (Active_Func != null)
            Active_Func.Invoke();
    }
}
