using PEPlugin;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using PEPlugin.Pmd;
using PEPlugin.View;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ManaLazyTool
{
    public partial class FormControl : Form
    {

        IPERunArgs Args { get; }
        IPXPmx Pmx { get; set; }
        public IPEPluginHost host;
        public IPEBuilder builder;
        public IPXPmxBuilder pmx_build;
        public IPEConnector connect;
        public IPEPMDViewConnector view;
        public IPXPmx pmx;
        public IList<IPXBone> bone;
        public IList<IPXVertex> vertex;
        public IList<IPXFace> face;
        public IList<IPXMorph> morph;
        public IList<IPXMaterial> material;
        public IList<IPXBody> body;
        public IPXHeader header;
        public IPXModelInfo info;
        public IPXModelInfo modelInfo;
        public JsonParsing jsonParsing;
        //public BoneManipulation boneManip;
        //public GeneralPMXEditor pmxGeneral;

        // global shit that is kinda important lol
        public string shaderPath = "";
        public string modelPath = "";
        public string texturePath = "" + @"\Textures\";
        public string materialPath = "" + @"\Materials\";
        public int ShaderOverrideState = 0; // default is 0, which is none
        public string shaderOverrideJsonPath = "";
        string includeFileName = "";
        bool shaderDirectory = false;
        bool saveEMDFile = false;
        public int shaderPresetState = 0;

        public void GetCurrentInfo()
        {
            pmx = connect.Pmx.GetCurrentState();
            bone = pmx.Bone;
            vertex = pmx.Vertex;
            header = pmx.Header;
            material = pmx.Material;
            body = pmx.Body;
            info = pmx.ModelInfo;
        }
        public void UpdateCurrentInfo()
        {
            connect.Pmx.Update(pmx); // update the info in pmx
            connect.Form.UpdateList(UpdateObject.All); // update form
            connect.View.PMDView.UpdateModel(); // refresh model ot reflect new changes
            connect.View.PMDView.UpdateView(); // refresh view to lect new changes
        }

        public FormControl(IPERunArgs args)
        {
            Args = args;
            host = Args.Host;
            builder = host.Builder;
            pmx_build = host.Builder.Pmx;
            connect = host.Connector;
            view = host.Connector.View.PMDView;

            InitializeComponent();
            Reload();
        }

        internal void Reload()
        {
            Pmx = Args.Host.Connector.Pmx.GetCurrentState();
        }

        private void FormControl_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/Manashiku");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Manashiku");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/hoyotoon");
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void connectBones_Click(object sender, EventArgs e)
        {
            try
            {
                BoneManipulation boneManip = new BoneManipulation();
                GetCurrentInfo();
                boneManip.AutoConnectStandardBones(bone);
                boneManip.AutoConnectStandardBones(bone);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("connect bones button error: \n" + ex);
            }
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void armRotatonButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.FixArmRotations(bone);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Fixing Arm Rotation: \n" + ex);
            }
        }

        private void getNamesButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.GetBones(oldNameList, newNameBox, bone);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get bones: \n" + ex);
            }
        }

        private void clearNamesButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.ClearBones(oldNameList, newNameBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to clear bones: \n" + ex);
            }
        }

        private void saveBonesButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.SaveList(oldNameList, newNameBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save list: \n" + ex);
            }
        }

        private void loadBonesButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.LoadList(oldNameList, newNameBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load list: \n" + ex);
            }
        }

        private void updateNewButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.ChangeSelectedName(oldNameList, newNameBox, newNameInput);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed changing selected name: \n" + ex);
            }
        }

        private void renameBonesButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.RenameBones(oldNameList, newNameBox, bone);
                foreach(IPXBone pmxBone in bone)
                {
                    if(pmxBone.Name == "delete" || pmxBone.Name == "Delete" || pmxBone.Name == "DELETE")
                    {
                        bone.Remove(pmxBone);
                    }
                    else
                    {
                        break;
                    }
                }
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed renaming bones: \n" + ex);
            }
        }

        private void resetTreeButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.PopulateBoneTree(boneTreeView, bone);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to populate tree: \n" + ex);
            }
        }

        private void boneTreeView_DragDrop(object sender, DragEventArgs e)
        {

            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.BoneDragDrop(sender, e, bone);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to move node: \n" + ex);
            }
        }

        private void boneTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void boneTreeView_DragOver(object sender, DragEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView.GetNodeAt(targetPoint);

            if (targetNode != null)
            {
                treeView.SelectedNode = targetNode;
            }
        }

        private void boneTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void applyTreeButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.ApplyHierarchyChanges(boneTreeView);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to apply hierarchy changes: \n" + ex);
            }
        }

        private void fixBonesButton_Click(object sender, EventArgs e)
        {

            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                List<string> boneList = new List<string>();
                boneManip.ConvertTreeToList(boneTreeView.Nodes, boneList);

                boneManip.ReorderBones(boneList, bone);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to fix hierarchy: \n" + ex);
            }
        }

        private void clearTreeButton_Click(object sender, EventArgs e)
        {
            boneTreeView.BeginUpdate();
            boneTreeView.Nodes.Clear();
            boneTreeView.EndUpdate();
        }

        private void IKLegsButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.AutoLegIKBones(bone, pmx_build);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to creat Leg IKs: \n" + ex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                GetCurrentInfo();
                int[] boneInd = view.GetSelectedBoneIndices();

                for (int i = 0; i < boneInd.Length; i++)
                {

                    bool useOffset = false;

                    MessageBox.Show(boneInd[i].ToString());
                    if (((bone[boneInd[i]].ToBone == null) && bone[boneInd[i]].ToOffset == Vector3.Zero) || !bone[boneInd[i]].Visible)
                    {
                        MessageBox.Show("Bone {0} is either invisible, has not bone end, or bone end offset", bone[boneInd[i]].Name);
                        return;
                    }
                    else if (bone[boneInd[i]].ToBone != null)
                    {
                        useOffset = false;
                    }
                    else if (bone[boneInd[i]].ToOffset != Vector3.Zero)
                    {
                        useOffset = true;
                    }

                    IPXBody newBody = pmx_build.Body();
                    body.Add(newBody);
                    IPXBone fakeBone = bone[boneInd[i]];
                    newBody.Name = bone[boneInd[i]].Name;
                    newBody.BoxKind = PEPlugin.Pmd.BodyBoxKind.Box;


                    V3 boneEnd = useOffset ? bone[boneInd[i]].ToOffset : bone[boneInd[i]].ToBone.Position;

                    newBody.Position = (bone[boneInd[i]].Position + boneEnd) * 0.5f;
                    newBody.BoxSize = new V3(0.5f, 0.5f, 0.75f);

                    V3 vecAxis = bone[boneInd[i]].Position - boneEnd;
                    Matrix matrix = Matrix.Identity;

                 

                    IPXBone bodyBone = bone[boneInd[i]];

                    if(vecAxis.Y < 0f)
                    {
                        vecAxis = -vecAxis;
                    }

                    Vector3 axisAngle = Vector3.Cross(vecAxis, Vector3.UnitZ);
                    axisAngle.Normalize();
                    Vector3 axisAngleB = Vector3.Cross(axisAngle, vecAxis);
                    axisAngleB.Normalize();

                    matrix.M11 = axisAngle.X;
                    matrix.M12 = axisAngle.Y;
                    matrix.M13 = axisAngle.Z;
                    matrix.M21 = vecAxis.X;
                    matrix.M22 = vecAxis.Y;
                    matrix.M23 = vecAxis.Z;
                    matrix.M31 = axisAngleB.X;
                    matrix.M32 = axisAngleB.Y;
                    matrix.M33 = axisAngleB.Z;
                    Vector3 position = bodyBone.Position;
                    matrix.M41 = position.X;
                    matrix.M42 = position.Y;
                    matrix.M43 = position.Z;


                    newBody.Rotation = MatrixToEuler_YZX(matrix);

                    MessageBox.Show("what");
                }


                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to creat Leg IKs: \n" + ex);
            }
        }

        public static Vector3 MatrixToEuler_ZXY0( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            if (m.M32 == 1f)
            {
                zero.X = 1.5707964f;
                zero.Z = (float)Math.Atan2((double)m.M21, (double)m.M11);
            }
            else if (m.M32 == -1f)
            {
                zero.X = -1.5707964f;
                zero.Z = (float)Math.Atan2((double)m.M21, (double)m.M11);
            }
            else
            {
                zero.X = -(float)Math.Asin((double)m.M32);
                zero.Y = -(float)Math.Atan2((double)(-(double)m.M31), (double)m.M33);
                zero.Z = -(float)Math.Atan2((double)(-(double)m.M12), (double)m.M22);
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_ZXY( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.X = -(float)Math.Asin((double)m.M32);
            if (zero.X == 1.5707964f || zero.X == -1.5707964f)
            {
                zero.Y = (float)Math.Atan2((double)(-(double)m.M13), (double)m.M11);
            }
            else
            {
                zero.Y = (float)Math.Atan2((double)m.M31, (double)m.M33);
                zero.Z = (float)Math.Asin((double)(m.M12 / (float)Math.Cos((double)zero.X)));
                if (m.M22 < 0f)
                {
                    zero.Z = 3.1415927f - zero.Z;
                }
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_XYZ( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.Y = -(float)Math.Asin((double)m.M13);
            if (zero.Y == 1.5707964f || zero.Y == -1.5707964f)
            {
                zero.Z = (float)Math.Atan2((double)(-(double)m.M21), (double)m.M22);
            }
            else
            {
                zero.Z = (float)Math.Atan2((double)m.M12, (double)m.M11);
                zero.X = (float)Math.Asin((double)(m.M23 / (float)Math.Cos((double)zero.Y)));
                if (m.M33 < 0f)
                {
                    zero.X = 3.1415927f - zero.X;
                }
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_YZX( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.Z = -(float)Math.Asin((double)m.M21);
            if (zero.Z == 1.5707964f || zero.Z == -1.5707964f)
            {
                zero.X = (float)Math.Atan2((double)(-(double)m.M32), (double)m.M33);
            }
            else
            {
                zero.X = (float)Math.Atan2((double)m.M23, (double)m.M22);
                zero.Y = (float)Math.Asin((double)(m.M31 / (float)Math.Cos((double)zero.Z)));
                if (m.M11 < 0f)
                {
                    zero.Y = 3.1415927f - zero.Y;
                }
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_ZXY_Lim2( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.X = -(float)Math.Asin((double)m.M32);
            if (zero.X == 1.5707964f || zero.X == -1.5707964f)
            {
                zero.Y = (float)Math.Atan2((double)(-(double)m.M13), (double)m.M11);
            }
            else
            {
                if (1.5358897f < zero.X)
                {
                    zero.X = 1.5358897f;
                }
                else if (zero.X < -1.5358897f)
                {
                    zero.X = -1.5358897f;
                }
                zero.Y = (float)Math.Atan2((double)m.M31, (double)m.M33);
                zero.Z = (float)Math.Asin((double)(m.M12 / (float)Math.Cos((double)zero.X)));
                if (m.M22 < 0f)
                {
                    zero.Z = 3.1415927f - zero.Z;
                }
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_XYZ_Lim2( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.Y = -(float)Math.Asin((double)m.M13);
            if (zero.Y == 1.5707964f || zero.Y == -1.5707964f)
            {
                zero.Z = (float)Math.Atan2((double)(-(double)m.M21), (double)m.M22);
            }
            else
            {
                if (1.5358897f < zero.Y)
                {
                    zero.Y = 1.5358897f;
                }
                else if (zero.Y < -1.5358897f)
                {
                    zero.Y = -1.5358897f;
                }
                zero.Z = (float)Math.Atan2((double)m.M12, (double)m.M11);
                zero.X = (float)Math.Asin((double)(m.M23 / (float)Math.Cos((double)zero.Y)));
                if (m.M33 < 0f)
                {
                    zero.X = 3.1415927f - zero.X;
                }
            }
            return zero;
        }

        public static Vector3 MatrixToEuler_YZX_Lim2( Matrix m)
        {
            Vector3 zero = Vector3.Zero;
            zero.Z = -(float)Math.Asin((double)m.M21);
            if (zero.Z == 1.5707964f || zero.Z == -1.5707964f)
            {
                zero.X = (float)Math.Atan2((double)(-(double)m.M32), (double)m.M33);
            }
            else
            {
                if (1.5358897f < zero.Z)
                {
                    zero.Z = 1.5358897f;
                }
                else if (zero.Z < -1.5358897f)
                {
                    zero.Z = -1.5358897f;
                }
                zero.X = (float)Math.Atan2((double)m.M23, (double)m.M22);
                zero.Y = (float)Math.Asin((double)(m.M31 / (float)Math.Cos((double)zero.Z)));
                if (m.M11 < 0f)
                {
                    zero.Y = 3.1415927f - zero.Y;
                }
            }
            return zero;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            shaderPresetState = comboBox.SelectedIndex;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            modelPath = modelPathInputBox.Text;
        }

        private void shaderPathGetButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.ValidateNames = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                // dlg.InitialDirectory = ;
                dlg.FileName = "";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    shaderPathInputBox.Text = Path.GetDirectoryName(dlg.FileName);
                    shaderPath = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }

        private void shaderPathInputBox_TextChanged(object sender, EventArgs e)
        {
            shaderPath = shaderPathInputBox.Text;
        }

        private void modelPathGetButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.ValidateNames = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                // dlg.InitialDirectory = ;
                dlg.FileName = "";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    modelPathInputBox.Text = Path.GetDirectoryName(dlg.FileName);
                    modelPath = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }

        private void materialPathInputBox_TextChanged(object sender, EventArgs e)
        {
            materialPath = materialPathInputBox.Text;
        }

        private void texturePathInputBox_TextChanged(object sender, EventArgs e)
        {
            texturePath = texturePathInputBox.Text;
        }

        private void materialPathGetButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.ValidateNames = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                dlg.InitialDirectory = Path.GetDirectoryName(modelPath);
                dlg.FileName = "";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    materialPathInputBox.Text = Path.GetDirectoryName(dlg.FileName);
                    materialPath = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }

        private void texturePathGetButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.ValidateNames = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                dlg.InitialDirectory = Path.GetDirectoryName(modelPath);
                dlg.FileName = "";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    texturePathInputBox.Text = Path.GetDirectoryName(dlg.FileName);
                    texturePath = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            string newPath = materialPath + @"\ShaderMateirals";
            Directory.CreateDirectory(newPath);
            if(shaderDirectory)
            {
                newPath = shaderPath;
            }
            else if( shaderDirectory && ( ( shaderPath == "" ) || !Directory.Exists( shaderPath ) ) )
            {
                MessageBox.Show("Invalid Shader Directory");
                return;
            }

            JsonParsing jsonParser = new JsonParsing();

            jsonParser.ParseAndConvertJsons(materialPath, newPath, shaderOverrideJsonPath, ShaderOverrideState, includeFileName);

            Process.Start(new ProcessStartInfo() { FileName = newPath, UseShellExecute = true });
        }

        private void ApplyJsonButton_Click(object sender, EventArgs e)
        {
            JsonParsing jsonParser = new JsonParsing();
            GetCurrentInfo();

            string relativePath = GetRelativePath(modelPath, texturePath);

            jsonParser.ApplyJsonToMaterials(materialPath, relativePath, texturePath, material, shaderPresetState);
            UpdateCurrentInfo();
        }

        public string GetRelativePath(string fromPath, string toPath)
        {
            string relativePath = Path.GetDirectoryName(fromPath);
            if(toPath.Contains(relativePath))
            {
                return toPath.Substring(relativePath.Length + 1);
            }
            else if(toPath == relativePath)
            {
                return "";
            }
            else
            {
                MessageBox.Show("Couldn't get relative path, using absolute instead");
                return toPath;
            }
        }

        private void resetPathsButton_Click(object sender, EventArgs e)
        {
            GetCurrentInfo();
            MessageBox.Show(pmx.FilePath);
            shaderPath = "";
            shaderPathInputBox.Text = shaderPath;
            modelPath = pmx.FilePath;
            modelPathInputBox.Text = modelPath;
            materialPath = Path.GetDirectoryName(modelPath) + @"\Materials\";
            materialPathInputBox.Text = materialPath;
            texturePath = Path.GetDirectoryName(modelPath) + @"\Textures\";
            texturePathInputBox.Text = texturePath;
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            try
            {
                JsonParsing jsonParser = new JsonParsing();

                jsonParser.CopyFoundTextures(materialPath, texturePath, shaderPath);

                MessageBox.Show("Finished Copying Shader Textures");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to copy textures: " + ex);
            }
        }

        private void overrideImportButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.ValidateNames = false;
                dlg.CheckFileExists = false;
                dlg.CheckPathExists = true;
                dlg.FileName = "Override Json or Txt file";
                dlg.Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    shaderOverrideJsonPath = Path.GetFullPath(dlg.FileName);
                    textBox1.Text = shaderOverrideJsonPath;
                    ShaderOverrideState = 1;
                    //MessageBox.Show(shaderOverrideJsonPath);
                }
            }
        }

        private void includeNameBox_TextChanged(object sender, EventArgs e)
        {
            includeFileName = includeNameBox.Text;
        }

        private void shaderDirectoryBool_CheckedChanged(object sender, EventArgs e)
        {
            shaderDirectory = !shaderDirectory;
            MessageBox.Show(shaderDirectory.ToString());
        }

        private void eyeBoneButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.CreateEyeBones(bone, pmx_build);
                UpdateCurrentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create Eye Bone: \n" + ex);
            }
        }

        private void autoEndButton_Click(object sender, EventArgs e)
        {
            BoneManipulation boneManip = new BoneManipulation();
            try
            {
                GetCurrentInfo();
                boneManip.AutoBoneEnds(bone, view.GetSelectedBoneIndices());
                UpdateCurrentInfo();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to set bone ends: \n" + ex);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            saveEMDFile = !saveEMDFile;
        }

        private void genEMDButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (shaderPath.Length == 0 || !Directory.Exists(shaderPath))
                {
                    MessageBox.Show("Please make sure to enter a valid shader directory.");
                    return;
                }
                GetCurrentInfo();
                JsonParsing jsonParser = new JsonParsing();
                if (shaderPresetState == 1)
                {
                    jsonParser.CreateShinyColorsEMDFile(materialPath, modelPath, shaderPath, material, shaderPresetState);
                    jsonParser.CreateShadowDriverEMD(materialPath, modelPath, shaderPath, material);

                }
                else
                {
                    jsonParser.CreateEMDFile(materialPath, modelPath, shaderPath, material, shaderPresetState);
                }

                Process.Start(new ProcessStartInfo() { FileName = Path.GetDirectoryName(modelPath), UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create emd file: \n" + ex);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void shdObjGenButton_Click(object sender, EventArgs e)
        {
            try
            {
                ShaderObjects shaderObject = new ShaderObjects();
                GetCurrentInfo();
                shaderObject.ShinyColorShaderObjects(material, pmx_build, bone);
                UpdateCurrentInfo();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed while modifying model" + ex);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            shaderOverrideJsonPath = textBox1.Text;
        }
    }
}