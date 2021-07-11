using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class ObjBaseTool
{
    public static void SetProperty(object target, string propertyName, string fieldValue)
    {
        Type type = target.GetType();

        PropertyInfo propertyInfo = type.GetProperty(propertyName);
        
        
        if (propertyInfo != null)
        {
            Type proType = propertyInfo.PropertyType;

            if (IsType(propertyInfo.PropertyType, "System.String"))
            {
                propertyInfo.SetValue(target, fieldValue, null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Boolean"))
            {
                propertyInfo.SetValue(target, Boolean.Parse(fieldValue), null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Int32"))
            {
                if (fieldValue != "")
                    propertyInfo.SetValue(target, int.Parse(fieldValue), null);
                else
                    propertyInfo.SetValue(target, 0, null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Single"))
            {
                if (fieldValue != "")
                    propertyInfo.SetValue(target, Single.Parse(fieldValue), null);
                else
                    propertyInfo.SetValue(target, 0, null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Double"))
            {
                if (fieldValue != "")
                    propertyInfo.SetValue(target, (float)Double.Parse(fieldValue), null);
                else
                    propertyInfo.SetValue(target, 0, null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Decimal"))
            {
                if (fieldValue != "")
                    propertyInfo.SetValue(target, Decimal.Parse(fieldValue), null);
                else
                    propertyInfo.SetValue(target, new Decimal(0), null);
                return;
            }

            if (IsType(propertyInfo.PropertyType, "System.Nullable`1[System.DateTime]"))
            {
                if (fieldValue != "")
                {
                    try
                    {
                        propertyInfo.SetValue(
                            target,
                            (DateTime?)DateTime.ParseExact(fieldValue, "yyyy-MM-dd HH:mm:ss", null), null);
                    }
                    catch
                    {
                        propertyInfo.SetValue(target, (DateTime?)DateTime.ParseExact(fieldValue, "yyyy-MM-dd", null), null);
                    }
                }
                else
                    propertyInfo.SetValue(target, null, null);
                return;
            }
        }
        else
        {
            FieldInfo fieldInfo = type.GetField(propertyName);
            if(fieldInfo != null)
            {
                Type proType = fieldInfo.FieldType;

                if (IsType(proType, "System.String"))
                {
                    fieldInfo.SetValue(target, fieldValue);
                    return;
                }

                if (IsType(proType, "System.Boolean"))
                {
                    fieldInfo.SetValue(target, Boolean.Parse(fieldValue));
                    return;
                }

                if (IsType(proType, "System.Int32"))
                {
                    if (fieldValue != "")
                        fieldInfo.SetValue(target, int.Parse(fieldValue));
                    else
                        fieldInfo.SetValue(target, 0);
                    return;
                }

                if(IsType(proType, "System.Single"))
                {
                    if (fieldValue != "")
                        fieldInfo.SetValue(target, Single.Parse(fieldValue));
                    else
                        fieldInfo.SetValue(target, 0);
                    return;
                }

                if (IsType(proType, "System.Double"))
                {
                    if (fieldValue != "")
                        fieldInfo.SetValue(target, (float)Double.Parse(fieldValue));
                    else
                        fieldInfo.SetValue(target, 0);
                    return;
                }

                if (IsType(proType, "System.Decimal"))
                {
                    if (fieldValue != "")
                        fieldInfo.SetValue(target, Decimal.Parse(fieldValue));
                    else
                        fieldInfo.SetValue(target, new Decimal(0));
                    return;
                }

                if (IsType(proType, "System.Nullable`1[System.DateTime]"))
                {
                    if (fieldValue != "")
                    {
                        try
                        {
                            fieldInfo.SetValue(
                                target,
                                (DateTime?)DateTime.ParseExact(fieldValue, "yyyy-MM-dd HH:mm:ss", null));
                        }
                        catch
                        {
                            fieldInfo.SetValue(target, (DateTime?)DateTime.ParseExact(fieldValue, "yyyy-MM-dd", null));
                        }
                    }
                    else
                        fieldInfo.SetValue(target, null);
                    return;
                }
            }
        }
    }

    public static void SetProperty(object target, string propertyName, object fieldValue)
    {
        Type type = target.GetType();

        PropertyInfo propertyInfo = type.GetProperty(propertyName);

        if (propertyInfo != null)
        {
            Debug.Log("1111111");
            Type proType = propertyInfo.PropertyType;

            if (IsType(proType, fieldValue))
            {
                Debug.Log("！！！！！！！！！！");
                Debug.Log(propertyName);

                propertyInfo.SetValue(target, fieldValue);
            }
        }
        else
        {
            FieldInfo info = type.GetField(propertyName);
            if (info != null)
            {
                Debug.Log("xxxxxxxxxx");

                Type proType = info.FieldType;

                if (IsType(proType, fieldValue))
                {
                    Debug.Log("！！！！！！！！！！");
                    Debug.Log(propertyName);

                    propertyInfo.SetValue(target, fieldValue);
                }
            }
        }
    }


    public static bool IsType(Type type, string typeName)
    {
        if (type.ToString() == typeName)
            return true;
        if (type.ToString() == "System.Object")
            return false;

        return IsType(type.BaseType, typeName);
    }

    public static bool IsType(Type type, object obj)
    {
        string typeName = obj.GetType().Name;
        
        if (type.ToString() == typeName)
            return true;
        if (type.ToString() == "System.Object")
            return false;

        return IsType(type.BaseType, typeName);
    }

    static int i = 0;
    public static string PrintObj(object obj)
    {
        i = 0;
        string print = GetObjPrintString(obj, 0);
        Debug.Log(print);
        return print;
    }

    public static string PrintObj(string objName, object obj)
    {
        i = 0;
        string print = GetObjPrintString(obj, 0);
        Debug.Log(objName +" = "+ print);
        return print;
    }

    static string GetObjPrintString(object obj, int i)
    {
        i++;
        if(i > 10)
        {
            return " is bed end ";
        }
        Type type = obj.GetType();
        
        if (type.IsPrimitive || type.IsEnum
            || !type.IsClass)
        {
            return obj.ToString();
        }

        // info.GetValue(obj)
        string print = "";
        if (type.IsArray)
        {
            MethodInfo methodInfo = type.GetMethod("GetEnumerator");
            IEnumerator itor = (IEnumerator)methodInfo.Invoke(obj, null);

            print += "[";
            while (itor.MoveNext())
            {
                object o = itor.Current;
                print += GetObjPrintString(o, i) + ",";
            }
            print += "]";
            return print;
        }

        foreach (PropertyInfo info in type.GetProperties())
        {
            print += "{" + info.Name + ":" + GetObjPrintString(info.GetValue(obj), i) + "}  ";
        }

        foreach (FieldInfo info in type.GetFields())
        {
            print += "{" + info.Name + ":" + GetObjPrintString(info.GetValue(obj), i) + "}  ";
        }
        
        return print;
    }
}
