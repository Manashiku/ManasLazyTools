using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEPlugin;
using SlimDX;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Runtime;

namespace ManaLazyTool
{
    public class BoneManipulation
    {
        #region Bone Dictionarys
        private Dictionary<string, string> standardBones = new Dictionary<string, string>
        {
            // spines
            { "上半身", "上半身2" },
            { "上半身2", "首" },
            { "首", "頭" },
            // left arm
            { "左肩", "左腕" },
            { "左腕", "左ひじ" },
            { "左ひじ", "左手首" },
            // right arm
            { "右肩", "右腕" },
            { "右腕", "右ひじ" },
            { "右ひじ", "右手首" },
            // left leg
            { "左足", "左ひざ" },
            { "左ひざ", "左足首" },
            { "左足首", "左足先EX" },
            // right leg
            { "右足", "右ひざ" },
            { "右ひざ", "右足首" },
            { "右足首", "右足先EX" },
            // left fingers
            { "左親指０", "左親指１" },
            { "左親指１", "左親指２" },
            { "左人指１", "左人指２" },
            { "左人指２", "左人指３" },
            { "左中指１", "左中指２" },
            { "左中指２", "左中指３" },
            { "左薬指１", "左薬指２" },
            { "左薬指２", "左薬指３" },
            { "左小指１", "左小指２" },
            { "左小指２", "左小指３" },
            // right fingers
            { "右親指０", "右親指１" },
            { "右親指１", "右親指２" },
            { "右人指１", "右人指２" },
            { "右人指２", "右人指３" },
            { "右中指１", "右中指２" },
            { "右中指２", "右中指３" },
            { "右薬指１", "右薬指２" },
            { "右薬指２", "右薬指３" },
            { "右小指１", "右小指２" },
            { "右小指２", "右小指３" },
        };

        private Dictionary<string, string> specialBones = new Dictionary<string, string>
        {
            { "上半身", "上半身1" },
            { "上半身1", "上半身2" },
        };

        private Dictionary<string, string> autoEndBones = new Dictionary<string, string>
        {
            // key = parent
            // value = child
            {"上半身", "下半身"},
            {"左親指１", "左親指２"},
            {"左人指２", "左人指３"},
            {"左中指２", "左中指３"},
            {"左薬指２", "左薬指３"},
            {"左小指２", "左小指３"},
            {"右親指１", "右親指２"},
            {"右人指２", "右人指３"},
            {"右中指２", "右中指３"},
            {"右薬指２", "右薬指３"},
            {"右小指２", "右小指３"},
            {"首", "頭"},
            {"右ひじ", "右手首"},
            {"左ひじ", "左手首"},
            {"左足首", "左足先EX"},
            {"右足首", "右足先EX"},
        };

        private Dictionary<string, string> autoEndBones_Fixer = new Dictionary<string, string>
        {
            // key = parent
            // value = child
            {"左中指１", "左手首"},
            {"右中指１", "右手首"},
        };

        private Dictionary<string, string> armBones = new Dictionary<string, string>
        {
            {"左腕", "左ひじ" },
            {"左ひじ", "左手首" },
            {"右腕", "右ひじ" },
            {"右ひじ", "右手首" },
        };

        private Dictionary<string, string> legBones = new Dictionary<string, string>
        {
            // left legs
            {"左足", "左ひざ" },
            {"左ひざ", "左足首" },
            {"左足首", "左足先EX" },
            // right legs
            {"右足", "右ひざ" },
            {"右ひざ", "右足首" },
            {"右足首", "右足先EX" },
        };
        #endregion
        public IList<IPXBone> bone;
        public IPXPmxBuilder pmxBuild;
        #region General Fucntions

        public void CreateEyeBones(IList<IPXBone> pmxBone, IPXPmxBuilder builder)
        {
            IPXBone eyeL = null;
            IPXBone eyeR = null;
            IPXBone headBone = null;

            string eyeBoneName = "両目";

            foreach(IPXBone bone in pmxBone)
            {
                if (bone.Name == "左目") { eyeL = bone; }
                else if (bone.Name == "右目") { eyeR = bone; }
                else if (bone.Name == "頭") { headBone = bone; }
                else { continue; }
            }

            if(eyeL == null || eyeR == null || headBone == null)
            {
                MessageBox.Show("Unable to find 左目, 右目, and/or 頭");
                return;
            }

            IPXBone eyeBone = builder.Bone();
            pmxBone.Add(eyeBone);
            
            eyeBone.Name = eyeBoneName;
            eyeBone.Parent = headBone;
            V3 offset = new V3(0.0f, 3.0f, 0.0f);
            eyeBone.Position = headBone.Position + offset;
            eyeBone.ToOffset = new V3(0.0f, 0.0f, -1.0f);
            

            eyeL.IsAppendRotation = true;
            eyeL.AppendParent = eyeBone;
            eyeL.AppendRatio = 1f;
            eyeL.Level = 2;
            eyeR.IsAppendRotation = true;
            eyeR.AppendParent = eyeBone;
            eyeR.AppendRatio = 1f;
            eyeR.Level = 2;

        }

        public void AutoConnectStandardBones(IList<IPXBone> pmxBone)
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in standardBones)
                {

                    foreach (IPXBone boneKey in pmxBone)
                    {
                        if (boneKey.Name == kvp.Key)
                        {

                            foreach (IPXBone boneValue in pmxBone)
                            {
                                if (boneValue.Name == kvp.Value)
                                {
                                    boneKey.ToBone = boneValue;
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
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex);
            }
        }

        public void AutoConnectSpecialBones(IList<IPXBone> pmxBone)
        {
            foreach (KeyValuePair<string, string> kvp in specialBones)
            {

                foreach (IPXBone boneKey in pmxBone)
                {
                    if (boneKey.Name == kvp.Key)
                    {
                        foreach (IPXBone boneValue in pmxBone)
                        {
                            if (boneValue.Name == kvp.Value)
                            {
                                boneKey.ToBone = boneValue;
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

        public void CalculateLocalAxis(IPXBone boneKey, IPXBone boneValue)
        {
            V3 vecAxis = boneValue.Position - boneKey.Position;
            
            V3 LocalX = new V3(0, 0, 0);
            V3 LocalZ = new V3(0, 0, 0);
            if (vecAxis == Vector3.UnitZ)
            {
                LocalX = Vector3.UnitZ;
                LocalZ = -Vector3.UnitX;
            }
            else if (vecAxis == -Vector3.UnitZ)
            {
                LocalX = -Vector3.UnitZ;
                LocalZ = Vector3.UnitX;
            }
            else
            {
                Matrix matrix = Matrix.RotationQuaternion(Q.Dir(vecAxis, Vector3.UnitZ, Vector3.UnitX, Vector3.UnitZ));
                LocalX = Vector3.TransformNormal(Vector3.UnitX, matrix);
                LocalZ = Vector3.TransformNormal(Vector3.UnitZ, matrix);
            }
            vecAxis.Normalize();
            V3 localX = Vector3.Zero;
            V3 localY = Vector3.Zero;
            V3 localZ = Vector3.Zero;
            boneKey.SetLocalAxis(LocalX, LocalZ);
            boneKey.IsLocalFrame = true;
            boneKey.GetLocalAxis(out localX, out localY, out localZ);
        }
        public void FixArmRotations(IList<IPXBone> pmxBone)
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in armBones)
                {
                    foreach (IPXBone boneKey in pmxBone)
                    {
                        if (boneKey.Name == kvp.Key)
                        {
                            foreach (IPXBone boneValue in pmxBone)
                            {
                                if (boneValue.Name == kvp.Value)
                                {
                                    // first connect the bone to their children as its necessary for setting the local axis
                                    boneKey.ToBone = boneValue;
                                    V3 vecAxis = boneValue.Position - boneKey.Position;
                                    vecAxis.Normalize();
                                    V3 LocalX = new V3(0, 0, 0);
                                    V3 LocalZ = new V3(0, 0, 0);
                                    if (vecAxis == Vector3.UnitZ)
                                    {
                                        LocalX = Vector3.UnitZ;
                                        LocalZ = -Vector3.UnitX;
                                    }
                                    else if (vecAxis == -Vector3.UnitZ)
                                    {
                                        LocalX = -Vector3.UnitZ;
                                        LocalZ = Vector3.UnitX;
                                    }
                                    else
                                    {
                                        Matrix matrix = Matrix.RotationQuaternion(Q.Dir(vecAxis, Vector3.UnitZ, Vector3.UnitX, Vector3.UnitZ));
                                        LocalX = Vector3.TransformNormal(Vector3.UnitX, matrix);
                                        LocalZ = Vector3.TransformNormal(Vector3.UnitZ, matrix);
                                    }

                                    boneKey.SetLocalAxis(LocalX, LocalZ);
                                    boneKey.IsLocalFrame = true;
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
            catch (Exception ex)
            {
                MessageBox.Show("Error doing something" + ex);
            }
        }

        public void AutoLegIKBones(IList<IPXBone> pmxBone, IPXPmxBuilder builder)
        {
            // various bone strings
            string[] ikBoneNames = { "左足ＩＫ", "右足ＩＫ", "左つま先ＩＫ", "右つま先ＩＫ" };
            string[] legBoneNames =
             {
                "左足", "左ひざ", "左足首", "左足先EX",
                "右足", "右ひざ", "右足首", "右足先EX"
             };

            // intialize bones as null first 
            IPXBone legL = null;
            IPXBone kneeL = null;
            IPXBone ankleL = null;
            IPXBone toeL = null;
            IPXBone toeEXL = null;
            IPXBone legR = null;
            IPXBone kneeR = null;
            IPXBone ankleR = null;
            IPXBone toeR = null;
            IPXBone toeEXR = null;
            IPXBone master = null;

            bool leftFlag = false;
            bool rightFlag = false;

            bool leftLevel = false;
            bool rightLevel = false;


            // check if bones exist
            foreach (IPXBone bone in pmxBone)
            {
                if (bone.Name == "全ての親") master = bone; // first find the mother bone, this one isnt required since the parents of the leg IKs will instead default to -1 then
                if (bone.Name == legBoneNames[0]) { legL = bone; }
                else if (bone.Name == legBoneNames[1]) { kneeL = bone; }
                else if (bone.Name == legBoneNames[2]) { ankleL = bone; }
                else if (bone.Name == legBoneNames[4]) { legR = bone; }
                else if (bone.Name == legBoneNames[5]) { kneeR = bone; }
                else if (bone.Name == legBoneNames[6]) { ankleR = bone; }
                else if (bone.Name == "左つま先") { toeL = bone; leftFlag = true; }
                else if (bone.Name == "右つま先") { toeR = bone; rightFlag = true; }
                else if (bone.Name == legBoneNames[3]) { toeEXL = bone; leftLevel = true; }
                else if (bone.Name == legBoneNames[7]) { toeEXR = bone; rightLevel = true; }

            }
            // first check if the legs exist 
            if ((legL == null || kneeL == null || ankleL == null) || (legR == null || kneeR == null || ankleR == null))
            {
                MessageBox.Show("Failed to find leg bones");
                return;
            }

            #region create toe bones
            if(!leftFlag)
            {
                IPXBone toeBoneLHelper = builder.Bone();
                pmxBone.Add(toeBoneLHelper);
                toeBoneLHelper.Name = "toeBoneLHelper"; 
                Vector3 bonePosL = ankleL.Position;
                toeBoneLHelper.Position = bonePosL;
                toeBoneLHelper.Position.Y = 0.0f;
                IPXBone toeBoneL = builder.Bone();
                pmxBone.Add(toeBoneL);
                toeBoneL.Position = ToeSolver(ankleL, toeBoneLHelper);
                toeBoneL.Name = "左つま先";
                toeBoneL.Parent = ankleL;
                toeL = toeBoneL;
                pmxBone.Remove(toeBoneLHelper);
            }

            if(!rightFlag)
            {
                IPXBone toeBoneRHelper = builder.Bone();
                pmxBone.Add(toeBoneRHelper);
                Vector3 bonePosR = ankleR.Position;
                toeBoneRHelper.Position = bonePosR;
                toeBoneRHelper.Position.Y = 0.0f;
                toeBoneRHelper.Name = "toeBoneRHelper";
                IPXBone toeBoneR = builder.Bone();
                pmxBone.Add(toeBoneR);
                toeBoneR.Position = ToeSolver(ankleR, toeBoneRHelper);
                toeBoneR.Name = "右つま先";
                toeBoneR.Parent = ankleR;
                toeR = toeBoneR;
                pmxBone.Remove(toeBoneRHelper);
            }

            ankleL.ToBone = toeL;
            ankleR.ToBone = toeR;

            #endregion

            if (leftLevel) toeEXL.Level = 2;
            if (rightLevel) toeEXR.Level = 2;


            #region left leg ik
            IPXBone legIKL = builder.Bone();
            pmxBone.Add(legIKL);
            CreateIKLeg(legIKL, ikBoneNames, pmxBone, builder, legL, kneeL, ankleL, true);
            #endregion

            #region right leg ik
            IPXBone legIKR = builder.Bone();
            pmxBone.Add(legIKR);
            CreateIKLeg(legIKR, ikBoneNames, pmxBone, builder, legR, kneeR, ankleR, false);
            #endregion

            if ((toeL == null) || (toeR ==  null))
            {
                MessageBox.Show("Failed to find toe bones");
                return;
            }

            #region left ankle ik
            IPXBone ankleIKL = builder.Bone();
            pmxBone.Add(ankleIKL);
            CreateIKAnkle(ankleIKL, ikBoneNames, pmxBone, builder, ankleL, toeL, true);
            #endregion

            #region right ankle ik
            IPXBone ankleIKR = builder.Bone();
            pmxBone.Add(ankleIKR);
            CreateIKAnkle(ankleIKR, ikBoneNames, pmxBone, builder, ankleR, toeR, false);
            #endregion
            legIKL.Parent = (master == null) ? null : master; 
            legIKR.Parent = (master == null) ? null : master; 
            ankleIKL.Parent = legIKL;
            ankleIKR.Parent = legIKR;
        }

        public Vector3 ToeSolver(IPXBone ankleBone, IPXBone boneHelper)
        {
            Vector2 boneB = new Vector2(ankleBone.Position.Z, ankleBone.Position.Y);
            Vector2 boneA = new Vector2(boneHelper.Position.Z, boneHelper.Position.Y);

            // Calculate the vector AB
            Vector2 AB = new Vector2(boneB.X - boneA.X, boneB.Y - boneA.Y);

            // Calculate the length of AB
            float ABLength = AB.Length();

            // Calculate the x and y components of AC, considering a 45-degree angle
            float ACx = (float)(ABLength / Math.Sqrt(2));
            float ACy = (float)(ABLength / Math.Sqrt(2));

            // Determine the direction of AC based on the position of B relative to A
            float direction = AB.X >= 0 ? 1 : -1;

            // Calculate the coordinates of point C
            float cX = boneA.X + direction * ACx;
            float cY = boneA.Y + ACy;

            float value = cY + cX;
            if(value > 0)
            {
                value = -value;
            }

            // Return the coordinates of point C
            return new Vector3(ankleBone.Position.X, 0.0f, value);
        }

        public void CreateIKLeg(IPXBone IKBone, string[] names, IList<IPXBone> pmxBone, IPXPmxBuilder builder, IPXBone leg, IPXBone knee, IPXBone ankle, bool leftRight)
        {
            //pmxBone.Add(IKBone);
            IKBone.Name = leftRight ? names[0] : names[1];
            IKBone.Position = new V3(ankle.Position.X, ankle.Position.Y, ankle.Position.Z);
            IKBone.IsIK = true;
            IKBone.IK.Target = ankle;
            IKBone.IK.LoopCount = 90;
            IKBone.IK.Angle = 115;
            IKBone.IsRotation = true;
            IKBone.IsRotation = true;
            IKBone.IsTranslation = true;
            IKBone.Visible = true;
            IKBone.Controllable = true;
            IKBone.ToOffset = new V3(0f, 0f, 1f);
            IPXIKLink legLinkA = builder.IKLink();
            legLinkA.Bone = knee;
            legLinkA.IsLimit = true;
            legLinkA.Low = new V3(Q.D2R(-180f), 0f, 0f);
            legLinkA.High = new V3(Q.D2R(-0.5f), 0f, 0f);
            IKBone.IK.Links.Add(legLinkA);
            IPXIKLink legLinkB = builder.IKLink();
            legLinkB.Bone = leg;
            IKBone.IK.Links.Add(legLinkB);
        }

        public void CreateIKAnkle(IPXBone IKBone, string[] names, IList<IPXBone> pmxBone, IPXPmxBuilder builder, IPXBone ankle, IPXBone toe, bool leftRight)
        {
            //pmxBone.Add(IKBone);
            IKBone.Name = leftRight ? names[2] : names[3];
            IKBone.Position = new V3(toe.Position.X, toe.Position.Y, toe.Position.Z);
            IKBone.IsIK = true;
            IKBone.IK.Target = toe;
            IKBone.IK.LoopCount = 90;
            IKBone.IK.Angle = 115;
            IKBone.IsRotation = true;
            IKBone.IsTranslation = true;
            IKBone.Visible = true;
            IKBone.Controllable = true;
            IKBone.ToOffset = new V3(0f, 0f, -1f);
            IPXIKLink ankleIKLink = builder.IKLink();
            ankleIKLink.Bone = ankle;
            IKBone.IK.Links.Add(ankleIKLink);
        }
       
        public void AutoBoneEnds(IList<IPXBone> pmxBone, int[] boneIndices)
        {
            for (int i = 0; i < boneIndices.Length; i++)
            {
                IPXBone ipxBone = pmxBone[boneIndices[i]];
                IPXBone ipxBone2 = ipxBone.Parent;
                IPXBone ipxBone3 = null;
                bool flag = false;

                if(ipxBone2.Parent != null)
                {
                    ipxBone3 = ipxBone2.Parent;
                    flag = true;
                }

                float parentLength = flag ? Vector3.Distance(ipxBone2.Position, ipxBone3.Position) * 0.5f : 0.5f;
                float childLength = Vector3.Distance(ipxBone2.Position, ipxBone.Position);

                float vLength = (parentLength + childLength) / 2.0f;

                V3 v = ipxBone.Position - ipxBone2.Position;
                v = v * vLength;

                pmxBone[boneIndices[i]].ToOffset = v;
                pmxBone[boneIndices[i]].ToBone = null;
            }

        }
        #endregion
        #region Bone Renaming
        public void GetBones(ListBox oldNamesListBox, ListBox newNamesListBox, IList<IPXBone> pmxBone)
        {
            // clear names first so the list doesnt get added onto over and over again
            oldNamesListBox.Items.Clear();
            newNamesListBox.Items.Clear();
            foreach (IPXBone bone in pmxBone)
            {
                oldNamesListBox.Items.Add(bone.Name);
                newNamesListBox.Items.Add(bone.Name);
            }
        }

        public void ClearBones(ListBox oldNamesListBox, ListBox newNamesListBox)
        {
            oldNamesListBox.Items.Clear();
            newNamesListBox.Items.Clear();
        }

        public void SaveList(ListBox oldNamesListBox, ListBox newNamesListBox)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    int omittedCount = 0;
                    List<string> linesToWrite = new List<string>();

                    for (int i = 0; i < oldNamesListBox.Items.Count; i++)
                    {
                        string oldName = oldNamesListBox.Items[i].ToString();
                        string newName = newNamesListBox.Items[i].ToString();
                        if (oldName != newName)
                        {
                            linesToWrite.Add(oldName + "=" + newName);
                        }
                        else
                        {
                            omittedCount++;
                        }
                    }

                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("// " + omittedCount + " bones with unchanged names were omitted");
                        foreach (string line in linesToWrite)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
            }
        }

        public void LoadList(ListBox oldNamesListBox, ListBox newNamesListBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    oldNamesListBox.Items.Clear();
                    newNamesListBox.Items.Clear();
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!line.TrimStart().StartsWith("//"))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length == 2)
                                {
                                    oldNamesListBox.Items.Add(parts[0]);
                                    newNamesListBox.Items.Add(parts[1]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ChangeSelectedName(ListBox oldNamesListBox, ListBox newNamesListBox, TextBox inputTextBox)
        {
            int selectedIndex = oldNamesListBox.SelectedIndex;

            if (selectedIndex != -1 && !string.IsNullOrEmpty(inputTextBox.Text))
            {
                newNamesListBox.Items[selectedIndex] = inputTextBox.Text;
            }
            else if (selectedIndex == -1)
            {
                MessageBox.Show("Please select a bone to rename.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (string.IsNullOrEmpty(inputTextBox.Text))
            {
                MessageBox.Show("Please enter a new name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        public void RenameBones(ListBox oldNamesListBox, ListBox newNamesListBox, IList<IPXBone> pmxBone)
        {
            for (int n = 0; n < oldNamesListBox.Items.Count; n++)
            {
                string oldName = oldNamesListBox.Items[n].ToString();
                string newName = newNamesListBox.Items[n].ToString();

                foreach (IPXBone bone in pmxBone)
                {
                    if (bone.Name == oldName)
                    {
                        bone.Name = newName;
                        break;
                    }
                }
            };
            // after it has finished renaming, update the model
            MessageBox.Show("Done", "Rename Bones");
        }
        #endregion
        #region Bone Hierarchy
        public void PopulateBoneTree(TreeView treeView, IList<IPXBone> pmxBone)
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            Dictionary<IPXBone, TreeNode> boneNodes = new Dictionary<IPXBone, TreeNode>();

            foreach (IPXBone bone in pmxBone)
            {
                TreeNode node = new TreeNode(bone.Name)
                {
                    Tag = bone
                };
                boneNodes[bone] = node;
            }

            // Add nodes to the tree view in the order they appear in pmxBone
            for (int i = 0; i < pmxBone.Count; i++)
            {
                IPXBone bone = pmxBone[i];
                if (bone.Parent == null)
                {
                    treeView.Nodes.Add(boneNodes[bone]);
                }
                else
                {
                    boneNodes[bone.Parent].Nodes.Add(boneNodes[bone]);
                }

                treeView.ExpandAll();
                treeView.EndUpdate();
            }
        }

        private bool IsDescendant(TreeNode potentialParent, TreeNode potentialDescendant)
        {
            while (potentialDescendant != null)
            {
                if (potentialDescendant.Parent == potentialParent)
                    return true;
                potentialDescendant = potentialDescendant.Parent;
            }
            return false;
        }

        public void BoneDragDrop(object sender, DragEventArgs e, IList<IPXBone> pmxBone)
        {
            TreeView treeView = (TreeView)sender;
            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = treeView.GetNodeAt(targetPoint);
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (targetNode != null && draggedNode != null && targetNode != draggedNode)
            {
                // Check if the target node is a descendant of the dragged node
                if (IsDescendant(draggedNode, targetNode))
                {
                    MessageBox.Show("Cannot drag a parent onto its child.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                IPXBone draggedBone = (IPXBone)draggedNode.Tag;
                IPXBone targetBone = (IPXBone)targetNode.Tag;

                treeView.BeginUpdate();

                if (targetNode.Parent == draggedNode.Parent)
                {
                    // Reordering within the same parent
                    int targetIndex = targetNode.Index;
                    TreeNode parentNode = targetNode.Parent;

                    draggedNode.Remove();
                    if (parentNode == null)
                    {
                        treeView.Nodes.Insert(targetIndex, draggedNode);
                    }
                    else
                    {
                        parentNode.Nodes.Insert(targetIndex, draggedNode);
                    }

                    // Update the order in pmxBone
                    int draggedIndex = pmxBone.IndexOf(draggedBone);
                    int newIndex = pmxBone.IndexOf(targetBone);
                    pmxBone.RemoveAt(draggedIndex);
                    pmxBone.Insert(newIndex, draggedBone);
                }
                else
                {
                    // Moving to a different parent
                    draggedBone.Parent = targetBone;
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                    targetNode.Expand();

                    UpdateBoneLevels(draggedBone, pmxBone);
                }

                treeView.EndUpdate();
            }
        }

        private void UpdateBoneLevels(IPXBone bone, IList<IPXBone> pmxBone)
        {
            foreach (IPXBone childBone in pmxBone.Where(b => b.Parent == bone))
            {
                UpdateBoneLevels(childBone, pmxBone);
            }
        }

        public void ApplyHierarchyChanges(TreeView treeView)
        {
            // Update the pmx model with the current tree view structure
            UpdateBoneHierarchy(treeView.Nodes);
            MessageBox.Show("Bone hierarchy changes have been applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateBoneHierarchy(TreeNodeCollection nodes, IPXBone parent = null)
        {
            foreach (TreeNode node in nodes)
            {
                IPXBone bone = (IPXBone)node.Tag;
                bone.Parent = parent;
                UpdateBoneHierarchy(node.Nodes, bone);
            }
        }

        public void ConvertTreeToList(TreeNodeCollection nodes, List<string> list)
        {
            foreach (TreeNode node in nodes)
            {
                list.Add(node.Text);
                if (node.Nodes.Count > 0)
                {
                    ConvertTreeToList(node.Nodes, list);
                }
            }
        }

        public void ReorderBones(List<string> boneNames, IList<IPXBone> pmxBone)
        {
            List<int> boneOrderList = new List<int>();

            foreach (string boneName in boneNames)
            {
                int boneIndex = pmxBone.IndexOf(pmxBone.FirstOrDefault(b => b.Name == boneName));
                if (boneIndex != -1)
                {
                    boneOrderList.Add(boneIndex);
                }
            }

            if (boneOrderList.Count == pmxBone.Count)
            {
                List<IPXBone> reorderedBones = boneOrderList.Select(index => pmxBone[index]).ToList();
                for (int i = 0; i < reorderedBones.Count; i++)
                {
                    pmxBone[i] = reorderedBones[i];
                }
            }
            else
            {
                MessageBox.Show("Error: Bone count mismatch", "Reorder Bones", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
