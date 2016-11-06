using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
public class sEvent : sSingleton<sEvent>
{
    public struct Pair
    {
        public object obj;
        public string funcname;
        public System.Reflection.MethodInfo method;
    };

    private Dictionary<string, List<Pair>> events = new Dictionary<string, List<Pair>>();

    public void clear()
    {
        events.Clear();
    }

    public void register(string eventname, object obj, string funcname)
    {
        //Debug.Log("regist:" + eventname);
        List<Pair> lst = null;

        Pair pair = new Pair();
        pair.obj = obj;
        pair.funcname = funcname;
        pair.method = obj.GetType().GetMethod(funcname);
        if (pair.method == null)
        {
            Debug.LogWarning("method is null:" + funcname);
            return;
        }

        if (!events.TryGetValue(eventname, out lst))
        {
            lst = new List<Pair>();
            lst.Add(pair);
            //Dbg.DEBUG_MSG("Event::register: event(" + eventname + ")!");
            events.Add(eventname, lst);
            return;
        }
        lst.Add(pair);
    }

    public void unregister(string eventname, object obj)
    {
        //Debug.Log("unregister:" + eventname);
        List<Pair> lst = null;
        if (events.TryGetValue(eventname, out lst))
        {
            for( int i = 0; i < lst.Count; ++i )
            {
                if( lst[i].obj == obj )
                {
                    lst.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public void callEvent(string eventname, params object[] objs)
    {
        //Debug.Log("callEvent:" + eventname);
        List<Pair> lst = null;
        if (events.TryGetValue(eventname, out lst))
        {
            //Debug.Log("call!");
            for (int i = 0; i < lst.Count; ++i)
            {
                lst[i].method.Invoke(lst[i].obj, objs);
            }
        }
    }
}
