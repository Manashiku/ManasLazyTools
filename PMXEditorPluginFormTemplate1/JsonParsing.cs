using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using System.Numerics;
using System.Windows;
using System.Drawing;

namespace ManaLazyTool
{
    public class JsonParsing
    {
        #region Json Classes
        public class SavedProperties
        {
            public Dictionary<string, TextureEnv> m_TexEnvs { get; set; }
            public Dictionary<string, int> m_Ints { get; set; }
            public Dictionary<string, float> m_Floats { get; set; }
            public Dictionary<string, Vector4> m_Colors { get; set; }
        }

        public class TextureEnv
        {
            public Texture m_Texture { get; set; }
            public Vector2 m_Scale { get; set; }
            public Vector2 m_Offset { get; set; }
        }

        public class Texture
        {
            public int m_FileID { get; set; }
            public long m_PathID { get; set; }
            public string Name { get; set; }
            public bool IsNull { get; set; }
        }

        public class Vector2
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        public class Vector4
        {
            public float r { get; set; }
            public float g { get; set; }
            public float b { get; set; }
            public float a { get; set; }
        }

        public class RootObject
        {
            public SavedProperties m_SavedProperties { get; set; }
            public string m_Name { get; set; }
        }
        #endregion

        #region Json Override Classes
        public class ShaderSettingsRoot
        {
            public ShaderSettings m_ShaderSettings { get; set; }
        }

        public class ShaderSettings
        {
            public string[] m_Header { get; set; }
            public Dictionary<string, Category> m_Categories { get; set; }
            public string[] m_Footer { get; set; }
        }

        public class Category
        {
            public int m_CategoryIndex { get; set; }
            public string[] m_CategoryHeader { get; set; }
            public string[] m_CategoryMapping { get; set; }
            public Dictionary<string, ParameterOverride> m_ParameterOverride { get; set; }
            public Dictionary<string, CustomParameter> m_CustomParameters { get; set; }
        }

        public class ParameterOverride
        {
            public string NameOverride { get; set; }
            public ValueOverride ValueOverride { get; set; }
        }

        public class ValueOverride
        {
            public string m_Path { get; set; }
            public float m_Float { get; set; }
            public Vector4 m_Vector { get; set; }
        }

        public class CustomParameter
        {
            public string DefaultValue { get; set; }
            public ChangeIf ChangeIf { get; set; }
        }

        public class ChangeIf
        {
            public FileNameContains FileNameContains { get; set; }
            public MaterialContains MaterialContains { get; set; }
            public MaterialContainsnt MaterialContainsnt { get; set; }
        }

        public class FileNameContains
        {
            public string Contains { get; set; }
            public string ChangeTo { get; set; }
        }

        public class MaterialContains
        {
            public string Contains { get; set; }
            public string ChangeTo { get; set; }
        }
        public class MaterialContainsnt
        {
            public string Contains { get; set; }
            public string ChangeTo { get; set; }
        }

        #endregion
        public void ParseAndConvertJsons(string materialPath, string newPath, string overridePath, int overrideState, string includeFileName)
        {

            if (overrideState <= 0)
            {

                // get files from directory first
                string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
                foreach (string file in jsonFiles)
                {
                    string readJson = File.ReadAllText(file);

                    StringBuilder shaderDefinitions = new StringBuilder();
                    RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(readJson);

                    string fileName = rootObject.m_Name;

                    foreach (var kvp in rootObject.m_SavedProperties.m_TexEnvs)
                    {
                        string textureName = kvp.Key;
                        TextureEnv texEnv = kvp.Value;

                        string commentStatus = texEnv.m_Texture.IsNull ? "// " : "";

                        if (texEnv.m_Texture.Name != "_MainTex") // main texture should be skipped
                        {
                            shaderDefinitions.AppendLine(commentStatus + "#define " + textureName + " \"" + @"textures/" + texEnv.m_Texture.Name + ".png\"");
                            shaderDefinitions.AppendLine(commentStatus + "#define " + textureName + "_ST " + "float4(" + texEnv.m_Scale.X + ", " + texEnv.m_Scale.Y + ", " + texEnv.m_Offset.X + ", " + texEnv.m_Offset.Y + ")");
                            shaderDefinitions.AppendLine("// " + textureName + "_NullStatus" + "\" " + texEnv.m_Texture.IsNull + "\"");
                            if ((texEnv.m_Texture.Name.Length == 0) && (texEnv.m_Texture.m_PathID != 0) && !texEnv.m_Texture.IsNull)
                            {
                                shaderDefinitions.AppendLine("// Couldn't find texture string in material: you may need to rerip it, reload the cab map in asset studio to fix relations, or manually fix it by loading textures via pathID");
                                shaderDefinitions.AppendLine("// pathID: " + texEnv.m_Texture.m_PathID.ToString());
                            }
                        }
                        else
                        {
                            return;
                        }

                    }

                    shaderDefinitions.AppendLine("\n");

                    foreach (var kvp in rootObject.m_SavedProperties.m_Floats)
                    {
                        string floatName = kvp.Key;
                        float floatValue = kvp.Value;

                        shaderDefinitions.AppendLine("#define " + floatName + " " + floatValue.ToString());
                    }

                    shaderDefinitions.AppendLine("\n");

                    foreach (var kvp in rootObject.m_SavedProperties.m_Colors)
                    {
                        string colorName = kvp.Key;
                        Vector4 color = kvp.Value;

                        shaderDefinitions.AppendLine("#define " + colorName + " float4(" + color.r + ", " + color.g + ", " + color.b + ", " + color.a + ")");
                    }

                    if (includeFileName != "") shaderDefinitions.AppendLine("\n#include \"" + includeFileName + "\"");
                    shaderDefinitions.AppendLine("\n// Created using Mana's Lazy Tools v0.0.0");


                    File.WriteAllText(newPath + @"\" + rootObject.m_Name + ".fx", shaderDefinitions.ToString());
                }
            }
            else
            {
                string readOverride;
                ShaderSettingsRoot shaderRoot;
                using (StreamReader reader = new StreamReader(overridePath))
                {
                    readOverride = reader.ReadToEnd();
                    shaderRoot = JsonConvert.DeserializeObject<ShaderSettingsRoot>(readOverride);
                }
                bool missingHeader = false;
                if (shaderRoot == null || shaderRoot.m_ShaderSettings.m_Header == null)
                {
                    missingHeader = true;
                    MessageBox.Show("Invalid shader settings: Header is missing");

                }

                // Process all JSON files in the material path
                string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
                foreach (string file in jsonFiles)
                {
                    ProcessShaderFile(file, shaderRoot.m_ShaderSettings, newPath, missingHeader);
                }
            }
        }

        private void ProcessShaderFile(string file, ShaderSettings shaderRoot, string newPath, bool missingHeader)
        {
            try
            {
                // Read and parse the material JSON file
                string readJson = File.ReadAllText(file);
                StringBuilder shaderDefinitions = new StringBuilder(4096); // Preallocate buffer for better performance
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(readJson);

                string fileName = rootObject.m_Name;
                if (!missingHeader)
                {
                    // Write header section
                    foreach (string header in shaderRoot.m_Header)
                    {
                        shaderDefinitions.AppendLine("//" + header);
                    }

                }

                shaderDefinitions.AppendLine("\n");

                // Process each category in the shader settings
                foreach (var category in shaderRoot.m_Categories)
                {
                    ProcessCategory(shaderDefinitions, category.Value, rootObject, fileName);
                }

                // Write footer section
                shaderDefinitions.AppendLine("\n");
                foreach (string foot in shaderRoot.m_Footer)
                {
                    string footLine = foot;
                    footLine = footLine.Replace("/'", "\"");
                    shaderDefinitions.AppendLine(footLine);
                }

                // Write the final shader file
                string outputPath = Path.Combine(newPath, rootObject.m_Name + ".fx");
                File.WriteAllText(outputPath, shaderDefinitions.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error processing file {0}: {1}", file, ex.Message));
            }
        }

        private void ProcessCategory(StringBuilder shaderDefinitions, Category category, RootObject rootObject, string fileName)
        {
            // Write category headers
            if (category.m_CategoryHeader != null)
            {
                foreach (string header in category.m_CategoryHeader)
                {
                    shaderDefinitions.AppendLine("// " + header);
                }
            }

            // Process mappings for this category
            if (category.m_CategoryMapping != null)
            {
                foreach (var mapping in category.m_CategoryMapping)
                {
                    ProcessTextureMapping(shaderDefinitions, mapping, rootObject, category.m_ParameterOverride);
                    ProcessFloatMapping(shaderDefinitions, mapping, rootObject, category.m_ParameterOverride);
                    ProcessColorMapping(shaderDefinitions, mapping, rootObject, category.m_ParameterOverride);
                }
            }

            // Process custom parameters
            ProcessCustomParameters(shaderDefinitions, category, rootObject, fileName);
        }

        private void ProcessTextureMapping(StringBuilder shaderDefinitions, string mapping, RootObject rootObject, Dictionary<string, ParameterOverride> parameterOverrides)
        {
            foreach (var kvp in rootObject.m_SavedProperties.m_TexEnvs)
            {
                string textureName = kvp.Key;
                TextureEnv texEnv = kvp.Value;

                // Skip main texture or non-matching textures
                if (texEnv.m_Texture.Name == "_MainTex" || textureName != mapping)
                {
                    continue;
                }

                string commentStatus = texEnv.m_Texture.IsNull ? "// " : "";

                // Apply any parameter overrides
                if (parameterOverrides != null)
                {
                    ParameterOverride paramOverride;
                    if (parameterOverrides.TryGetValue(textureName, out paramOverride))
                    {
                        texEnv.m_Texture.Name = paramOverride.ValueOverride.m_Path;
                        textureName = paramOverride.NameOverride;
                    }
                }

                // Write texture definitions
                shaderDefinitions.AppendLine(commentStatus + "#define " + textureName + " \"textures/" + texEnv.m_Texture.Name + ".png\"");
                shaderDefinitions.AppendLine(string.Format("{0}#define {1}_ST float4({2}, {3}, {4}, {5})",
                    commentStatus, textureName, texEnv.m_Scale.X, texEnv.m_Scale.Y, texEnv.m_Offset.X, texEnv.m_Offset.Y));
                shaderDefinitions.AppendLine("// " + textureName + "_NullStatus\" " + texEnv.m_Texture.IsNull + "\"");

                // Add warning for missing texture names
                if (string.IsNullOrEmpty(texEnv.m_Texture.Name) && texEnv.m_Texture.m_PathID != 0 && !texEnv.m_Texture.IsNull)
                {
                    shaderDefinitions.AppendLine("// Couldn't find texture string in material: you may need to rerip it, reload the cab map in asset studio to fix relations, or manually fix it by loading textures via pathID");
                    shaderDefinitions.AppendLine("// pathID: " + texEnv.m_Texture.m_PathID.ToString());
                }
            }
        }

        private void ProcessFloatMapping(StringBuilder shaderDefinitions, string mapping, RootObject rootObject, Dictionary<string, ParameterOverride> parameterOverrides)
        {
            foreach (var kvp in rootObject.m_SavedProperties.m_Floats)
            {
                string floatName = kvp.Key;
                float floatValue = kvp.Value;

                // Apply any parameter overrides
                if (parameterOverrides != null)
                {
                    ParameterOverride paramOverride;
                    if (parameterOverrides.TryGetValue(floatName, out paramOverride))
                    {
                        floatName = paramOverride.NameOverride;
                        floatValue = paramOverride.ValueOverride.m_Float;
                    }
                }

                // textures take priority 
                if (floatName == mapping && !(rootObject.m_SavedProperties.m_TexEnvs.ContainsKey(floatName)) && !(floatName == "_DropShadowEffect"))
                {
                    shaderDefinitions.AppendLine("#define " + floatName + " " + floatValue.ToString());
                }
            }
        }

        private void ProcessColorMapping(StringBuilder shaderDefinitions, string mapping, RootObject rootObject, Dictionary<string, ParameterOverride> parameterOverrides)
        {
            foreach (var kvp in rootObject.m_SavedProperties.m_Colors)
            {
                string colorName = kvp.Key;
                Vector4 color = kvp.Value;

                // Apply any parameter overrides
                if (parameterOverrides != null)
                {
                    ParameterOverride paramOverride;
                    if (parameterOverrides.TryGetValue(colorName, out paramOverride))
                    {
                        colorName = paramOverride.NameOverride;
                        color = paramOverride.ValueOverride.m_Vector;
                    }
                }

                if (colorName == mapping)
                {
                    shaderDefinitions.AppendLine(string.Format("#define {0} float4({1}, {2}, {3}, {4})",
                        colorName, color.r, color.g, color.b, color.a));
                }
            }
        }

        private void ProcessCustomParameters(StringBuilder shaderDefinitions, Category category, RootObject rootObject, string fileName)
        {
            if (category.m_CustomParameters == null) return;

            foreach (var parameter in category.m_CustomParameters)
            {
                string parameterName = parameter.Key;
                string parameterValue = parameter.Value.DefaultValue;


                // Skip if parameter already exists in the material properties
                if (rootObject.m_SavedProperties.m_TexEnvs.ContainsKey(parameter.Key) ||
                    rootObject.m_SavedProperties.m_Floats.ContainsKey(parameter.Key) ||
                    rootObject.m_SavedProperties.m_Colors.ContainsKey(parameter.Key))
                {
                    continue;
                }

                parameterValue = ConditionSolver(parameter.Value, fileName, rootObject, parameterValue);
                // remove /' and replace with "
                parameterValue = parameterValue.Replace("/'", "\"");

                // Skip writing the the custom parameter if it at all returns with the word omit

                if (!(parameterValue.IndexOf("omit", StringComparison.OrdinalIgnoreCase) >= 0)) shaderDefinitions.AppendLine("#define " + parameterName + " " + parameterValue);
            }
        }

        private string ConditionSolver(CustomParameter parameter, string fileName, RootObject rootObject, string defaultValue)
        {
            if (parameter.ChangeIf == null) return defaultValue;

            // Check filename conditions
            if (parameter.ChangeIf.FileNameContains != null &&
                fileName.Contains(parameter.ChangeIf.FileNameContains.Contains))
            {
                return parameter.ChangeIf.FileNameContains.ChangeTo;
            }

            // Check material contains conditions
            if (parameter.ChangeIf.MaterialContains != null)
            {
                bool containsProperty =
                    rootObject.m_SavedProperties.m_TexEnvs.ContainsKey(parameter.ChangeIf.MaterialContains.Contains) ||
                    rootObject.m_SavedProperties.m_Floats.ContainsKey(parameter.ChangeIf.MaterialContains.Contains) ||
                    rootObject.m_SavedProperties.m_Colors.ContainsKey(parameter.ChangeIf.MaterialContains.Contains);

                if (containsProperty)
                {
                    return parameter.ChangeIf.MaterialContains.ChangeTo;
                }
            }

            // Check material doesn't contain conditions
            if (parameter.ChangeIf.MaterialContainsnt != null)
            {
                bool containsProperty =
                    rootObject.m_SavedProperties.m_TexEnvs.ContainsKey(parameter.ChangeIf.MaterialContainsnt.Contains) ||
                    rootObject.m_SavedProperties.m_Floats.ContainsKey(parameter.ChangeIf.MaterialContainsnt.Contains) ||
                    rootObject.m_SavedProperties.m_Colors.ContainsKey(parameter.ChangeIf.MaterialContainsnt.Contains);

                if (!containsProperty)
                {
                    return parameter.ChangeIf.MaterialContainsnt.ChangeTo;
                }
            }

            return defaultValue;
        }
        public void CreateEMDFile(string materialPath, string modelPath, string shaderPath, IList<IPXMaterial> materials, int shaderPresetState)
        {
            string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
            StringBuilder emdFile = new StringBuilder();
            string fileName = Path.GetFileNameWithoutExtension(modelPath) + ".emd";
            string fileDirectory = Path.GetDirectoryName(modelPath);

            // this is important for the file
            emdFile.AppendLine("[Info]\nVersion = 3");
            emdFile.AppendLine("\n");
            emdFile.AppendLine("[Effect]\nObj = none\nObj.show = true");

            foreach (IPXMaterial mat in materials)
            {
                foreach (string file in jsonFiles)
                {
                    string fName = Path.GetFileNameWithoutExtension(file);
                    if (mat.Name == fName)
                    {
                        emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\" + fName + ".fx");
                    }

                }
            }

            File.WriteAllText(fileDirectory + @"\" + fileName, emdFile.ToString());
        }

        public void CreateShinyColorsEMDFile(string materialPath, string modelPath, string shaderPath, IList<IPXMaterial> materials, int shaderPresetState)
        {
            string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
            StringBuilder emdFile = new StringBuilder();
            string fileName = Path.GetFileNameWithoutExtension(modelPath) + ".emd";
            string fileDirectory = Path.GetDirectoryName(modelPath);

            // this is important for the file
            emdFile.AppendLine("[Info]\nVersion = 3");
            emdFile.AppendLine("\n");
            emdFile.AppendLine("[Effect]\nObj = none\nObj.show = true");

            foreach (IPXMaterial mat in materials)
            {
                foreach (string file in jsonFiles)
                {
                    string fName = Path.GetFileNameWithoutExtension(file);
                    if (mat.Name == fName)
                    {
                        emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\" + fName + ".fx");
                    }
                    else
                    {
                        continue;
                    }
                }

                if (mat.Name == "BodySphere" || mat.Name == "HeadSphere")
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "].show = false");
                }
                else if (mat.Name.Contains("+"))
                {
                    string outline_hair = "outline_materials_hair";
                    string outline_face = "outline_materials_only_face";
                    string outline_base = "outline_materials_only_standard";

                    string name = outline_base;
                    if (mat.Name.Contains("hair"))
                    {
                        name = outline_hair;
                    }
                    else if (mat.Name.Contains("face") || mat.Name.Contains("eye"))
                    {
                        name = outline_face;
                    }

                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\" + name + ".fx");
                }
            }

            File.WriteAllText(fileDirectory + @"\" + fileName, emdFile.ToString());
        }

        public void CreateShadowDriverEMD(string materialPath, string modelPath, string shaderPath, IList<IPXMaterial> materials)
        {
            StringBuilder emdFile = new StringBuilder();
            string fileName = Path.GetFileNameWithoutExtension(modelPath) + "_ShadowDriver" + ".emd";
            string fileDirectory = Path.GetDirectoryName(modelPath);

            // this is important for the file
            emdFile.AppendLine("[Info]\nVersion = 3");
            emdFile.AppendLine("\n");
            emdFile.AppendLine("[Effect]\nObj = none\nObj.show = true");
            foreach (IPXMaterial mat in materials)
            {
                string matName = mat.Name;
                if (matName.Contains("skin") || matName.Contains("dress"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_ShadowReciever_HeadBody.fxsub");
                }
                else if (matName.Contains("face") || matName.Contains("eye"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_ShadowReciever_Hair.fxsub");
                }
                else if (matName.Contains("HeadSphere"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_ShadowCasterA_Head.fxsub");
                }
                else if (matName.Contains("BodySphere"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_ShadowCasterA_Body.fxsub");
                }
                else if (matName.Contains("hair"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_ShadowCasterA_Hair.fxsub");
                }
                else if (matName.Contains("+"))
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "].show = false");
                }
                else
                {
                    emdFile.AppendLine("Obj[" + materials.IndexOf(mat) + "] = " + shaderPath + @"\ShadowsDriver\DropShadow_None.fxsub");
                }
                File.WriteAllText(fileDirectory + @"\" + fileName, emdFile.ToString());
            }
        }

        // scan and parse the materials for the texture names, find the textures in the texture folder
        // and then move the textures into shaderPath+"\textures\"
        public void CopyFoundTextures(string materialPath, string texturePath, string shaderPath)
        {
            string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
            foreach (string file in jsonFiles)
            {
                string readJson = File.ReadAllText(file);

                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(readJson);

                string fileName = rootObject.m_Name;

                foreach (var kvp in rootObject.m_SavedProperties.m_TexEnvs)
                {
                    string textureName = kvp.Key;
                    TextureEnv texEnv = kvp.Value;

                    if (texEnv.m_Texture.Name != "" || textureName != "_MainTex")
                    {
                        string textureFile = texturePath + @"\" + texEnv.m_Texture.Name + ".png";

                        if (File.Exists(textureFile))
                        {
                            string newPath = shaderPath + @"\textures";
                            Directory.CreateDirectory(newPath);

                            string newFileName = newPath + @"\" + texEnv.m_Texture.Name + ".png";
                            if (!File.Exists(newFileName))
                            {
                                File.Copy(textureFile, newFileName);
                            }
                        }
                    }
                }
            }
        }

        public void ApplyJsonToMaterials(string materialPath, string texturePath, string absoluteTexturePath, IList<IPXMaterial> pmxMaterial, int shaderPresetState)
        {
            string[] jsonFiles = Directory.GetFiles(materialPath, "*.json");
            foreach (string file in jsonFiles)
            {
                string readJson = File.ReadAllText(file);

                StringBuilder shaderDefinitions = new StringBuilder();
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(readJson);

                string fileName = rootObject.m_Name;
                foreach (IPXMaterial mat in pmxMaterial)
                {
                    if (fileName == mat.Name)
                    {
                        foreach (var kvp in rootObject.m_SavedProperties.m_TexEnvs)
                        {
                            if (kvp.Key == "_MainTex")
                            {
                                mat.Tex = texturePath + @"\" + kvp.Value.m_Texture.Name + ".png";
                            }
                            if(kvp.Key == "_OutlineTex" && shaderPresetState == 1)
                            {
                                if (kvp.Value.m_Texture.IsNull == false)
                                {
                                    MessageBox.Show(mat.Name + "/n" + texturePath);
                                    string filePath = absoluteTexturePath + @"\" + kvp.Value.m_Texture.Name + ".png";
                                    var faces = mat.Faces;
                                    var bitmap = getImageBitmap(filePath);
                                    foreach (var face in faces)
                                    {
                                        ApplyEdgeFromImage(face.Vertex1, bitmap);
                                        ApplyEdgeFromImage(face.Vertex2, bitmap);
                                        ApplyEdgeFromImage(face.Vertex3, bitmap);
                                    }
                                }
                                else { continue; }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        foreach (var kvp in rootObject.m_SavedProperties.m_Colors)
                        {
                            if (kvp.Key == "_Color")
                            {
                                mat.Diffuse = new V4(kvp.Value.r, kvp.Value.g, kvp.Value.b, kvp.Value.a);
                                mat.Ambient = mat.Diffuse.ToV3() / 2f;
                            }
                            else if (kvp.Key == "_OutlineColor" || kvp.Key == "_EdgeColor")
                            {
                                mat.EdgeColor = new V4(kvp.Value.r, kvp.Value.g, kvp.Value.b, kvp.Value.a);
                            }
                            else if (kvp.Key.Contains("Specular") || kvp.Key.Contains("Spec") || kvp.Key.Contains("Highlight"))
                            {
                                mat.Specular = new V3(kvp.Value.r, kvp.Value.g, kvp.Value.b);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        foreach (var kvp in rootObject.m_SavedProperties.m_Floats)
                        {
                            if (kvp.Key.Contains("Specular") || kvp.Key.Contains("Spec") || kvp.Key.Contains("Highlight"))
                            {
                                mat.Power = kvp.Value;
                            }
                            else if (kvp.Key.Contains("Outline") || kvp.Key.Contains("Edge"))
                            {

                                mat.EdgeSize = kvp.Value;
                                if (shaderPresetState == 1) mat.EdgeSize *= 420.2f;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

        }

        private void ApplyEdgeFromImage(IPXVertex vert, Bitmap bitmap)
        {
            var uv = vert.UV;
            int x = (int)Math.Round(normalize(uv.U) * (bitmap.Width - 1), 0);
            int y = (int)Math.Round(normalize(uv.V) * (bitmap.Height - 1), 0);
            var pixel = bitmap.GetPixel(x, y);
            vert.EdgeScale = convertRGBtoLuma(pixel.R, pixel.G, pixel.B);
        }
        private static float normalize(float value)
        {
            return value - (float)Math.Floor(value);
        }

        private static Bitmap getImageBitmap(string filePath)
        {
            Bitmap bmp;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bmp = new Bitmap(stream);
            }
            return bmp;
        }

        private static float convertRGBtoLuma(int red, int green, int blue)
        {
            // calc with CIE coefficient
            var calculated_val = (0.2126 * red + 0.7152 * green + 0.0722 * blue) / 255;
            var simplified_val = (calculated_val < 0.01) ? 0.0f
                : (calculated_val > 0.995) ? 1.0f : (float)Math.Round(calculated_val, 2);
            return simplified_val;
        }

        private class VertexPosition
        {
            float X { get; set; }
            float Y { get; set; }
            float Z { get; set; }

            public VertexPosition(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }

            public override bool Equals(object obj)
            {
                VertexPosition target = (VertexPosition)obj;
                return (this.X == target.X && this.Y == target.Y && this.Z == target.Z);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }
}
