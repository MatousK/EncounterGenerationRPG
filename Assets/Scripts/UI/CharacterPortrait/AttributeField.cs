using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AttributeField: MonoBehaviour
{
    public float ValueToShow;
    public Text Label;

    public void Start()
    {
        UpdateLabel();
    }

    public void Update()
    {
        UpdateLabel();
    }

    void UpdateLabel()
    {
        Label.text = ValueToShow.ToString();
    }
}