using UnityEngine;
using System.Collections;
using System;

public class TestScri : MonoBehaviour {

    private float m_countdown = 10;
    private bool m_turnOn = false;

    WeakReference m_wrf_b;
    TestA m_ta;

    // Use this for initialization
    void Start()
    {
        m_turnOn = true;

        TestB tb = new TestB();
        m_wrf_b = new WeakReference(tb);

        m_ta = new TestA(m_wrf_b);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_turnOn == false)
            return;

        if (m_countdown >= 0)
        {
            m_countdown -= Time.deltaTime;
        }
        else
        {
            m_turnOn = false;

            m_ta = null;

            GC.Collect();
            Debug.LogError("清理内存");
        }
    }

    public class TestA
    {
        private long[] m_array;

        WeakReference m_wrf;

        public TestA(WeakReference wrf)
        {
            m_array = new long[100000000];

            m_wrf = wrf;

            TestB tb = (TestB)m_wrf.Target;
            tb.TestFun();
        }
    }
    public class TestB
    {
        private long[] m_array;

        public TestB()
        {
            m_array = new long[100000000];
        }

        public void TestFun()
        {
            Debug.LogError("测试方法");
        }
    }
}
