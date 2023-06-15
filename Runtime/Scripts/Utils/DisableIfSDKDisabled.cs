using System;
using UnityEngine;

namespace Kitrum.GeeklabSDK
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableIfSDKDisabled : PropertyAttribute
    {
    }
}