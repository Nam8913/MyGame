using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ModAssembly
{
   public ModAssembly(ModContentPack modContentPack)
   {
      this.modContentPack = modContentPack;
   }
   public void LoadAssembly()
   {
      if(!globalResolveAssembly)
      {
         ResolveEventHandler target = (object obj, ResolveEventArgs args) => Assembly.GetExecutingAssembly();
         AppDomain.CurrentDomain.AssemblyResolve += target.Invoke;
         globalResolveAssembly = true;
      }
      if(modContentPack != null)
      {
         foreach(var path in modContentPack.getAllFileExtraDll())
         {
            FileInfo fileInfo = new FileInfo(path);
            Assembly assembly = null;
            try
            {
               byte[] rawData = File.ReadAllBytes(fileInfo.FullName);
               FileInfo fileInfo2 = new FileInfo(Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.FullName)) + ".pdb");
               if (fileInfo2.Exists)
               {
                  byte[] rawSymbolStore = File.ReadAllBytes(fileInfo2.FullName);
                  assembly = Assembly.Load(rawData, rawSymbolStore);
               }
               else
               {
                  assembly = Assembly.Load(rawData);
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Failed to load assembly {fileInfo.Name}: {ex.Message}");
               break;
            }
            if (!(assembly == null) && this.AssemblyIsUsable(assembly))
				{
					GenTypes.ClearCache();
					this.loadedAssemblies.Add(assembly);
				}
         }
      }
   }
   private bool AssemblyIsUsable(Assembly asm)
   {
      try
      {
         asm.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
         StringBuilder stringBuilder = new StringBuilder();
         stringBuilder.AppendLine(string.Concat(new object[]
         {
            "ReflectionTypeLoadException getting types in assembly ",
            asm.GetName().Name,
            ": ",
            ex
         }));
         stringBuilder.AppendLine();
         stringBuilder.AppendLine("Loader exceptions:");
         if (ex.LoaderExceptions != null)
         {
            foreach (Exception ex2 in ex.LoaderExceptions)
            {
               stringBuilder.AppendLine("   => " + ex2.ToString());
            }
         }
         Debug.LogError(stringBuilder.ToString());
         return false;
      }
      catch (Exception ex3)
      {
         Debug.LogError(string.Concat(new object[]
         {
            "Exception getting types in assembly ",
            asm.GetName().Name,
            ": ",
            ex3
         }));
         return false;
      }
      return true;
   }

   private ModContentPack modContentPack;
   public List<Assembly> loadedAssemblies = new List<Assembly>();
   public static bool globalResolveAssembly = false;
}
