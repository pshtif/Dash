/*
 *	Created by:  Peter @sHTiF Stefcek
 */
#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Editor;
using UnityEditor;

namespace Dash.Editor
{
    public class AOTGenerator
    {
        public static void GenerateDLL(bool p_generateLinkXml = true, bool p_includeOdin = false)
        {
            if (DashEditorCore.EditorConfig.scannedAOTTypes == null) DashEditorCore.EditorConfig.scannedAOTTypes = new List<Type>();
            if (DashEditorCore.EditorConfig.explicitAOTTypes == null) DashEditorCore.EditorConfig.explicitAOTTypes = new List<Type>();

            DashEditorCore.EditorConfig.AOTAssemblyGeneratedTime = DateTime.Now;
            
            if (p_generateLinkXml)
            {
                if (p_includeOdin)
                {
                    File.WriteAllText(DashEditorCore.EditorConfig.AOTAssemblyPath + "/link.xml",
                        @"<linker>                    
                         <assembly fullname=""" + DashEditorCore.EditorConfig.AOTAssemblyName + @""" preserve=""all""/>
                         <assembly fullname=""Dash"" preserve=""all""/>
                         <assembly fullname=""OdinSerializer"" preserve=""all""/>
                      </linker>");
                }
                else
                {
                    File.WriteAllText(DashEditorCore.EditorConfig.AOTAssemblyPath + "/link.xml",
                        @"<linker>                    
                         <assembly fullname=""" + DashEditorCore.EditorConfig.AOTAssemblyName + @""" preserve=""all""/>
                         <assembly fullname=""Dash"" preserve=""all""/>
                      </linker>");
                }
            }
            
            List<Type> aotTypes = DashEditorCore.EditorConfig.scannedAOTTypes.Concat(DashEditorCore.EditorConfig.explicitAOTTypes)
                .ToList();
            AOTSupportUtilities.GenerateDLL(DashEditorCore.EditorConfig.AOTAssemblyPath,
                DashEditorCore.EditorConfig.AOTAssemblyName, aotTypes, false);
        }
        
        public static void RemoveScannedAOTType(Type p_type)
        {
            DashEditorCore.EditorConfig.scannedAOTTypes.Remove(p_type);
            
            EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }
        
        public static void RemoveExplicitAOTType(Type p_type)
        {
            DashEditorCore.EditorConfig.explicitAOTTypes.Remove(p_type);
            
            EditorUtility.SetDirty(DashEditorCore.EditorConfig);
        }
    }
}
#endif