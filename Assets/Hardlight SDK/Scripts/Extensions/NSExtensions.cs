﻿/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using System;
using UnityEngine;

namespace Hardlight.SDK
{
	public static class Extensions
    {
        public static UnityEngine.Quaternion Normalize(this UnityEngine.Quaternion q)
        {
            float d = Mathf.Sqrt(q.w*q.w + q.x*q.x + q.y*q.y + q.z*q.z);
			
            return new UnityEngine.Quaternion(q.x/d, q.y/d, q.z/d, q.w/d);
        }

        public static UnityEngine.Quaternion Clone(this UnityEngine.Quaternion q)
        {
            return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static float Dot(this UnityEngine.Quaternion q, UnityEngine.Quaternion other)
        {
            return q.x * other.x + q.y * other.y + q.z * other.z + q.w * other.w;
        }
    }
}