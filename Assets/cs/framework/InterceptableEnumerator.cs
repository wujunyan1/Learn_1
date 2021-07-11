using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptableEnumerator : IEnumerator
{
    public object Current => enumerator.Current;

    IEnumerator enumerator;
    public delegate void CatchListenerDelegate(UObject o);
    public delegate bool CancelFuncDelegate();


    CatchListenerDelegate catchListener = null;
    CancelFuncDelegate cancelFunc = null;
    CatchListenerDelegate finallListener = null;

    bool isException = false;
    System.Exception exception = null;

    public InterceptableEnumerator(IEnumerator itor)
    {
        enumerator = itor;
    }

    public bool MoveNext()
    {
        if (isCancelTask())
        {
            return false;
        }

        bool isMoveNext = false;

        try
        {
            isMoveNext = enumerator.MoveNext();
        }
        catch (System.Exception e)
        {
            if (this.catchListener != null)
            {
                UObject o = UObjectPool.Get();
                o.Set("e", e);
                o.Set("itor", enumerator);

                foreach (System.Delegate dele in catchListener.GetInvocationList())
                {
                    //类型转换
                    CatchListenerDelegate delegateClass = (CatchListenerDelegate)dele;

                    //调用并 得到返回结果
                    delegateClass.Invoke(o);
                }
            }

            isException = true;
            exception = e;
        }

        return isMoveNext;
    }

    public void Reset()
    {
        enumerator.Reset();
    }

    bool isCancelTask()
    {
        if (isException)
        {
            IsFinally();
        }

        if (cancelFunc != null)
        {
            bool res = false;
            foreach (System.Delegate dele in cancelFunc.GetInvocationList())
            {
                //类型转换
                CancelFuncDelegate delegateClass = (CancelFuncDelegate)dele;

                //调用并 得到返回结果
                // 只要有一个true 同意取消，则取消
                res = res || delegateClass.Invoke();
            }

            return res;
        }
        return false;
    }

    void IsFinally()
    {
        if (this.finallListener != null)
        {
            UObject o = UObjectPool.Get();
            o.Set("itor", enumerator);
            o.Set("isException", isException);
            o.Set("exception", exception);

            foreach (System.Delegate dele in finallListener.GetInvocationList())
            {
                //类型转换
                CatchListenerDelegate delegateClass = (CatchListenerDelegate)dele;

                //调用并 得到返回结果
                delegateClass.Invoke(o);
            }
        }
    }

    public void AddCatchListener(CatchListenerDelegate catchListener)
    {
        if (this.catchListener == null)
        {
            this.catchListener = catchListener;
        }
        else
        {
            this.catchListener += catchListener;
        }
    }

    public void AddFinallListener(CatchListenerDelegate finallListener)
    {
        if (this.finallListener == null)
        {
            this.finallListener = finallListener;
        }
        else
        {
            this.finallListener += finallListener;
        }
    }

    public void AddCancelFunc(CancelFuncDelegate cancelFunc)
    {
        if (this.cancelFunc == null)
        {
            this.cancelFunc = cancelFunc;
        }
        else
        {
            this.cancelFunc += cancelFunc;
        }
    }
}

public static class InterceptableEnumeratorExtend
{
    public static InterceptableEnumerator WrapEnumerator(this IEnumerator itor)
    {
        InterceptableEnumerator enumerator = new InterceptableEnumerator(itor);

        enumerator.AddCatchListener(delegate(UObject o)
        {
            System.Exception e = (System.Exception)o.Get("e");

            Debug.LogError(e);
        });

        return enumerator;
    }
}