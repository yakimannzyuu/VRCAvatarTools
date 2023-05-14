using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yakimannzyuu
{
    // 2023年5月12日適当に移植
    public class BoneTransporter
    {
        [SerializeField]
        Transform avatar;
        [SerializeField]
        Transform child;
        [SerializeField]
        const string addName = "_BoneTransporter";

        void Update()
        {
            BoneTransport(
                avatar.Find("Armature"),
                child.Find("Armature"),
                addName
            );
        }
        public void BoneTransport(Transform avatar, Transform child, string addName)
        {
            SetParent(child, avatar, addName);
        }

        void SetParent(Transform _target, Transform _to, string _addName)
        {
            // Debug.Log(_target+">>>"+_to);
            if(_target.parent == _to)
            {
                return;
            }
            _target.SetParent(_to);
            
            for(int i = _target.childCount - 1; i >= 0; i--)
            {
                for(int j = 0; j < _to.childCount; j++)
                {
                    if(_target.GetChild(i).name.Contains(_to.GetChild(j).name))
                    // if(_target.GetChild(i).name == _to.GetChild(j).name)
                    {
                        SetParent(_target.GetChild(i), _to.GetChild(j), _addName);
                        break;
                    }
                }
            }
            _target.name += _addName;
            // _target.transform.localPosition = Vector3.zero;
        }
    }
}
