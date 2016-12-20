using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class TestOutArgF : MonoBehaviour {
    string luaStr= @"
            print('hello lua!')
            local box = UnityEngine.BoxCollider
            function func(ray)
               local _layer = 2 ^ LayerMask.NameToLayer('Default')
                local flag, hit = UnityEngine.Physics.Raycast(ray, nil, 5000, _layer)                          
                --local flag, hit = UnityEngine.Physics.Raycast(ray, RaycastHit.out, 5000, _layer)                
                UnityEngine.Debug.Log('pick from c#, point: [{0}, {1}, {2}]');
                if flag then
                    print('pick from lua, point: '..tostring(hit.point))                                        
                end
            end
        ";
    LuaState state = null;
    LuaFunction func = null;

	// Use this for initialization
	void Start () {
        Debug.Log("lua use C#!");
        state = new LuaState();
        LuaBinder.Bind(state);
        state.Start();
        state.DoString(luaStr);

        func = state.GetFunction("func");
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera camera = Camera.main;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool flag = Physics.Raycast(ray, out hit, 5000, 1 << LayerMask.NameToLayer("Default"));

            if (flag)
            {
                Debugger.Log("pick from c#, point: [{0}, {1}, {2}]", hit.point.x, hit.point.y, hit.point.z);
            }

            func.BeginPCall();
            func.Push(ray);
            func.PCall();
            func.EndPCall();
        }

        state.CheckTop();
        state.Collect();
    }

    void OnDestroy()
    {
        func.Dispose();
        func = null;

        state.Dispose();
        state = null;
    }
}
