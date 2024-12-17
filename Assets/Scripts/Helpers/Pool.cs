using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KoboldTools
{

    public class Pool<T> where T : Component
    {

        public Pool(T template, bool checkPoolComponents = false)
        {
            _template = template;
            _template.gameObject.SetActive(false);
            _checkPoolComponents = checkPoolComponents;
        }

        private bool _checkPoolComponents = false;
        private T _template;
        private Stack<T> _used = new Stack<T>();
        private Stack<T> _unused = new Stack<T>();

        public T pop()
        {
            if (_unused.Count > 0)
            {
                _used.Push(_unused.Pop());
            }
            else
            {
                T newObject = Object.Instantiate(_template) as T;
                newObject.transform.SetParent(_template.transform.parent, false);
                _used.Push(newObject);
            }

            if (_checkPoolComponents)
            {
                PoolPopEvent ppe = _used.Peek().GetComponent<PoolPopEvent>();
                if(ppe != null)
                {
                    ppe.onPop();
                }
            }

            return _used.Peek();
        }

        public void releaseAll()
        {
            while (_used.Count > 0)
            {
                T element = _used.Pop();
                if (element != null && element.gameObject != null)
                {
                    element.gameObject.SetActive(false);
                    _unused.Push(element);
                }
            }
        }

        public Stack<T> getUsed()
        {
            return _used;
        }

        public T[] getUsedAsArray()
        {
            return _used.ToArray();
        }

    }

    public class LinkedPool<T> where T : Component
    {

        public LinkedPool(T template, bool checkPoolComponents = false)
        {
            _template = template;
            _template.gameObject.SetActive(false);
            _checkPoolComponents = checkPoolComponents;
        }

        private bool _checkPoolComponents = false;
        private T _template;
        private LinkedList<T> _used = new LinkedList<T>();
        private LinkedList<T> _unused = new LinkedList<T>();

        public T pop()
        {
            if (_unused.Count > 0)
            {
                _used.AddLast(_unused.Last());
                _unused.RemoveLast();
            }
            else
            {
                T newObject = Object.Instantiate(_template) as T;
                newObject.transform.SetParent(_template.transform.parent, false);
                _used.AddLast(newObject);
            }

            if (_checkPoolComponents)
            {
                PoolPopEvent ppe = _used.Last().GetComponent<PoolPopEvent>();
                if (ppe != null)
                {
                    ppe.onPop();
                }
            }

            return _used.Last();
        }

        public void releaseOne(T element)
        {
            if(element == null)
            {
                Debug.LogWarning("[Pool] Try to remove null element. Will be ignored.");
                return;
            }

            foreach (T e in _used)
            {
                if (e == element)
                {
                    _unused.AddLast(e);
                }
            }
            _used.Remove(element);
            element.gameObject.SetActive(false);
        }

        public void releaseAll()
        {
            while (_used.Count > 0)
            {
                T element = _used.Last();
                _used.RemoveLast();
                if (element != null && element.gameObject != null)
                {
                    element.gameObject.SetActive(false);
                    _unused.AddLast(element);
                }
            }
        }

        public LinkedList<T> getUsed()
        {
            return _used;
        }

        public T[] getUsedAsArray()
        {
            return _used.ToArray();
        }

    }
}
