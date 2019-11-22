using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using MethodBody = dnlib.DotNet.Emit.MethodBody;

namespace ILPUnpacker{
    internal class Program{
        public static void Main(string[] args){
            Console.Title = "Test ILProtector Unpacker by TobitoFatito";
            Harmony.Patch();
            ModuleDefMD Module = ModuleDefMD.Load(args[0]);
            Assembly asm = Assembly.LoadFrom(args[0]);
            TypeDef globalType =Module.GlobalType;
            int num = 0;
            foreach (FieldDef fieldDef in globalType.Fields)
            {
                if (fieldDef.Name == "Invoke")
                {
                    num = fieldDef.MDToken.ToInt32();
                }
            }
            if (num == 0)
            {
                Console.WriteLine("[!] Couldn't find Invoke");
            }
            
            MethodInfo methodInfo = (MethodInfo) asm.EntryPoint;

            Console.WriteLine("Invoking!!!");
            if (methodInfo.GetParameters().Length == 0){
                methodInfo.Invoke(null, null);
            }
            else{
                methodInfo.Invoke(null,new object[]{args});
            }
           
            Console.WriteLine("Invoked!");
           
               

            foreach (var type in Module.GetTypes()){
                foreach (var methodd in type.Methods){
                    try{
                        if (methodd.HasBody && methodd.Body.HasInstructions){
                            if (methodd.Body.Instructions[0].OpCode == OpCodes.Ldsfld &&
                                methodd.Body.Instructions[0].ToString().Contains("Invoke")){
                                if (methodd.Body.Instructions[1].IsLdcI4()){
                                    var methoddd123 =
                                        ResolveMethodBodyShit(methodd.Body.Instructions[1].GetLdcI4Value(),Module);
                                    if (methoddd123 != null){
                                        methodd.FreeMethodBody();
                                        methodd.Body = methoddd123.Body;
                                    }
                                }
                            }
                        }
                    }
                    catch{
                        
                    }
                    

                }
            }

            Save(Module, args[0]);
            Console.ReadLine();
        }
        public static void Save(ModuleDefMD Module, string path){
            var nativeModuleWriterOptions = new ModuleWriterOptions(Module);
            nativeModuleWriterOptions.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
            nativeModuleWriterOptions.Logger = DummyLogger.NoThrowInstance;
            nativeModuleWriterOptions.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            nativeModuleWriterOptions.Cor20HeaderOptions.Flags = ComImageFlags.ILOnly;
            nativeModuleWriterOptions.MetadataOptions.PreserveHeapOrder(Module, true);
            Module.Write(
                Path.Combine(Path.GetDirectoryName(path),Path.GetFileNameWithoutExtension(path)+"-Unpacked.exe"),
                nativeModuleWriterOptions);
        }
        public static MethodDef ResolveMethodBodyShit(int num,ModuleDefMD Module){
            GCHandle obj = Harmony.Handlaaaa;
            try{
                var asd = obj.Target as object[];
                var f = asd[num].GetType();
                var s = f.GetRuntimeFields();
                var gh = s.ToArray()[1].GetValue(asd[num]) as Delegate[];
                var del = gh[0];

                DynamicMethodBodyReader reader = new DynamicMethodBodyReader(Module, del);
                reader.Read();
                return reader.GetMethod();
            }
            catch{
                
            }

            return null;
        }
    }
}