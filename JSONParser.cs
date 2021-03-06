﻿#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_6 || UNITY_7
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;


/** 
 *  Recursively parses JSON files or strings. 
 *  
 *  DOUBLE QUOTES ONLY. Escape as necessary. No char objects.
 *  
 *  JSON can otherwise be slightly invalid, but results could be unreliable if so (such as elements after error not being parsed, etc).
 *  Keep in mind that in JSON anything in double quotes are strings, so only primitives outside quotes are converted to correct type.
 *  
 *  All objects are converted to Dictionary{string, JSObject}, all arrays are converted to List{JSObject}, and all doubles are converted to floats
 *  
 *  The default debugger (Unity or System.Diagnostics) is invoked on error when debugging, but no exceptions are intentionally thrown at run time.
 *  
 *  Implemented using the 'JSONObject' library and my 'JSObject' library (both included in project)
 *  
 *  Usage: Standard square bracket syntax for dictionary, lists. 
 *      JSObject c = parse(JSONString);      
 *      c["character_name"];                                //Objects access using field name 
 *      c["expressions_list"]["exp_default"];               //Read JSON file to see format.         
 *      c["expressions_list"]["exp_angry"][0];               
 *      c["expressions_list"]["exp_confused"][0]["hi"];     //Arrays(Lists) access with number indices 
 *      
 *      c.ContainsKey("character_name");                    //Some methods and properties available directly 
 *      c.Keys; 
 *      ... 
 *  
 *      Dictionary{string, JSObject} dict = c;               //Assign to actual type for other methods and uses (foreach uses c.GetEnumerator()) 
 *      foreach (JSObject d in dict){ ... }                   
 */
public class JSONParser
{

    /**  
     *  Recursively parses JSON in given file, returning a JSObject object containing Dictionary{string, JSObject}, List{JSObject}, string, int, float, bool and null objects.
     *  
     *  @param filePath
     *  @returnA JSObject object representing the JSON data. 
     */
    public static JSObject parseFile(string filePath)
    {
        try
        {
            return parse(File.ReadAllText(filePath));
        }
        catch (Exception)
        {
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_6 || UNITY_7
            Debug.LogWarning
#else
                Debug.WriteLine
#endif
                        ("Unable to read file (is filepath correct?).");
        }
        return null;
    }

    /**  
     *  Recursively parses given JSONString, returning a JSObject object containing Dictionary{string, JSObject}, List{JSObject}, string, int, float, bool and null objects.
     *  
     *  @param JSONString
     *  @returnA JSObject object representing the JSON data. 
     */
    public static JSObject parse(string JSONString)
    {
        try
        {
            var c = new JSONObject(JSONString);
            return parse(new JSONObject(JSONString));
        }
        catch (Exception)
        {
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_6 || UNITY_7
            Debug.LogWarning
#else
                Debug.WriteLine
#endif
                        ("JSON parse error.");
        }
        return null;
    }

    /**  
     *  Recursively parses given JSONObject, returning a JSObject object containing Dictionary{string, JSObject}, List{JSObject}, string, int, float, bool and null objects.
     *  
     *  @param obj
     *  @returnA JSObject object representing the JSON data. 
     */
    public static JSObject parse(JSONObject obj)
    {
        try
        {
            switch (obj.type)
            {
                case JSONObject.Type.OBJECT:
                    Dictionary<string, JSObject> d = new Dictionary<string, JSObject>();
                    for (int i = 0; i < obj.list.Count; i++)
                    {
                        d.Add(obj.keys[i], parse(obj.list[i]));
                    }
                    return d;
                case JSONObject.Type.ARRAY:
                    List<JSObject> l = new List<JSObject>();
                    foreach (JSONObject j in obj.list)
                    {
                        l.Add(parse(j));
                    }
                    return l;
                case JSONObject.Type.STRING:
                    return obj.str;
                case JSONObject.Type.NUMBER:
                    if (obj.n % 1 == 0)
                    {
                        return (int)obj.n;
                    }
                    return obj.n;
                case JSONObject.Type.BOOL:
                    return obj.b;
                case JSONObject.Type.NULL:
                default:
                    return null;
            }
        }
        catch (Exception)
        {
#if UNITY_2 || UNITY_3 || UNITY_4 || UNITY_5 || UNITY_6 || UNITY_7
            Debug.LogWarning
#else
                Debug.WriteLine
#endif
                        ("JSON parse error.");
        }
        return null;
    }
}

