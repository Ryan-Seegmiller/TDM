using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Direction { up, right, down, left }
public static class MathFunc
{

    //Returns 1 if f is positive and -1 if negative
    public static float PosOrNeg(float f)
    {
        return (f >= 0) ? (1) : (-1);
    }

    //Returns true if 'position' is within 'area'
    public static bool IsInArea2D(Vector2 position, Rect area)
    {
        return (position.x < area.position.x + (area.size.x / 2)) && (position.x > area.position.x - (area.size.x / 2))
        && (position.y < area.position.y + (area.size.y / 2)) && (position.y > area.position.y - (area.size.y / 2));
    }

    #region Timers

    //Returns true if the time has past the given number of seconds past the given start time
    public static bool Timeout(float waitTime, float startTime)
    {
        return ((Time.realtimeSinceStartup - startTime) >= waitTime);
    }
    //Returns the amount of time left until this timeout occurs
    public static float TimeLeft(float waitTime, float startTime)
    {
        return (waitTime - (Time.realtimeSinceStartup - startTime));
    }

    public static IEnumerator Timer(float seconds, string method, GameObject obj)
    {
        float counter = seconds;
        while (counter > 0)
        {
            yield return null;
            //Tick down timer if unpaused
            if (GameManager.instance.gameState != GameState.Pause)
            {
                counter -= Time.deltaTime;
            }
        }

        //Call the method of the object
        obj.SendMessage(method);
    }
    public static IEnumerator Timer(float seconds, Action method)
    {
        float counter = seconds;
        while (counter > 0)
        {
            yield return null;
            //Tick down timer if unpaused
            if (GameManager.instance.gameState != GameState.Pause)
            {
                counter -= Time.deltaTime;
            }
        }

        //Call the method of the object
        method.Invoke();
    }

    #endregion

    #region Angles

    //Wraps an angle around if more than 360 or less than 0
    public static float ClampAngle(float angle)
    {
        if (angle < 0)
        { return angle + 360; }
        else if (angle >= 360)
        { return angle - 360; }
        return angle;
    }

    //Converts a Vector2 to an angle between 0 and 360 degrees, with Vector2.right being 0
    public static float Vector2Angle(Vector2 vector2)
    {
        if (vector2 == Vector2.zero)
        { return 0; }

        float returnAngle;
        if (vector2.x < 0)
        {
            returnAngle = 360 - (Mathf.Atan2(vector2.x, -vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            returnAngle = Mathf.Atan2(vector2.x, -vector2.y) * Mathf.Rad2Deg;
        }

        //Rotate -90 degrees so that 0 is to the right instead of up
        returnAngle = ClampAngle(returnAngle - 90);


        return returnAngle;
    }

    #endregion

    #region Arrays

    //Returns the provided array converted to a string
    public static string ArrayToString(object[] arr)
    {
        string s = "[";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i].ToString();
            if (i < arr.Length - 1)
            { s += ", "; }
        }
        s += "]";

        return s;
    }

    public static string ArrayToString(int[] arr)
    {
        string s = "[";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i].ToString();
            if (i < arr.Length - 1)
            { s += ", "; }
        }
        s += "]";

        return s;
    }

    //Returns the provided array with the indexes of its contents shuffled
    public static object[] ShuffleArray(object[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            object temp = arr[i];
            int j = UnityEngine.Random.Range(i, arr.Length);
            arr[i] = arr[j];
            arr[j] = temp;
        }

        return arr;
    }

    //Returns the index of the first instance found of a given value in an array
    public static int GetArrayIndex(object[] arr, object value)
    {
        return 0;
    }

    #endregion

}
