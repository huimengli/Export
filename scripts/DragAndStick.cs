using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Export.BehaviourEX;
using Export.Tools;
using UnityEngine;

class DragAndStick:DragAndStickBehaviour
{
    /// <summary>
    /// 接受点名称
    /// </summary>
    public string receptorPointName;

    /// <summary>
    /// 读取接受点
    /// </summary>
    private Regex readName;

    void Start()
    {
        readName = new Regex(receptorPointName);

        receptorPoints = transform.GetComponentsInChildren<Transform>();
        receptorPoints = receptorPoints.Where(e => e != null && readName.IsMatch(e.gameObject.name)).ToArray();
    }

    void Update()
    {
        Upgrade();
    }
}
