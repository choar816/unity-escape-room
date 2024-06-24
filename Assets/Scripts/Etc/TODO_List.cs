using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TODO_List : MonoBehaviour
{
    [Serializable]
    public class Check_Point
    {
        public string Work = "";
        public bool Do = false;
    }

    public List<Check_Point> TODO_Work;
}
