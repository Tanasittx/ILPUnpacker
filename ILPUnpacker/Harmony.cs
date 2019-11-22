﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Harmony;

namespace ILPUnpacker{
    internal static class Harmony{
        public static HarmonyInstance inst;
        public static void Patch(){
            inst = HarmonyInstance.Create("tobito.fatito");
            inst.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAl(){
            inst.UnpatchAll();

        }

        [HarmonyPatch(typeof(StackFrame), "GetMethod")]
        public class PatchStackTraceGetMethod{
            public static MethodInfo MethodToReplace;

            public static void Postfix(ref MethodBase __result){
                if (__result.Name.Contains("Invoke"))
                    //just replace it with a method
                    __result = MethodToReplace ?? MethodBase.GetCurrentMethod();
            }
        }
        [HarmonyPatch(typeof(GCHandle), "FromIntPtr")]
        public class PatchStackTraceGetMethod2{
            public static MethodInfo MethodToReplace;

            public static void Postfix(IntPtr value,ref IntPtr __result){
                if (!test.ContainsValue(value)){
                    test.Add(0, value);
                    UnpatchAl();
                    var shh = GCHandle.FromIntPtr(value);
                    Handlaaaa = shh;
                }
                else{
                    Console.WriteLine("sheesh");
                }
            }
        }

      

        public static GCHandle Handlaaaa;
        public static Dictionary<int,IntPtr> test = new Dictionary<int, IntPtr>();

        [HarmonyPatch(typeof(Assembly), "GetCallingAssembly")]
        public class PatchGetCallingAssembly{
            public static Assembly MethodToReplace;

            public static void Postfix(ref Assembly __result){
                if (__result == typeof(PatchGetCallingAssembly).Assembly)
                    //just replace it with a method
                    __result = MethodToReplace;
            }
        }
    }
}