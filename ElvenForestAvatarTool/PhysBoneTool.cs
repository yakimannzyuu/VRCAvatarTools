using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Yakimannzyuu
{
    public static class PhysBoneTool
    {
        static public VRCPhysBone[] GetAllPhysBones(GameObject obj)
        {
            return obj.GetComponentsInChildren<VRCPhysBone>(true);
        }
        static public void AddIgnore(VRCPhysBone pb, Transform tra)
        {
            foreach(Transform ignored in pb.ignoreTransforms)
                if(ignored == tra)
                    return;
            pb.ignoreTransforms.Add(tra);
        }
        

        public static void AllChildrenIgnorePB(VRCPhysBone pb, Transform tra)
        {
            AddIgnore(pb, tra);
            for(int i = 0; i < tra.childCount; i++)
            {
                AllChildrenIgnorePB(pb, tra.GetChild(i));
            }
        }
    }
}
