using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Best
{
    public class GlobalObjectComponent:MonoBehaviour
    {
        void Update()
        {
            GlobalObject.Instance._DispatchUpdate();
        }

        public void FixedUpdate()
        {
            GlobalObject.Instance._DispathFixedUpdate();
        }
        void LateUpdate()
        {
            GlobalObject.Instance._DispatchLateUpdate();
        }
        void OnApplicationPause(bool pauseStatus)
        {
            GlobalObject.Instance._DispatchApplicationPause(pauseStatus);
        }
    }
}
