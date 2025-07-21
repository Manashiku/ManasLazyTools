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

namespace ManaLazyTool
{
    class BodyManipulation
    {
        public IList<IPXBone> bone;
        public IList<IPXBody> body;
        public IPXPmxBuilder pmxBuild;

        public void CreateBodyAndRotate(IPXBone bodyBone, IList<IPXBody> bodyList, IPXPmxBuilder builder)
        {

            if(((bodyBone.ToBone != null) && bodyBone.ToOffset != Vector3.Zero) || bodyBone.Visible)
            {
                MessageBox.Show("Bone {0} is either invisible, has not bone end, or bone end offset", bodyBone.Name);
                return;
            }

            IPXBody newBody = builder.Body();
            bodyList.Add(newBody);
            newBody.Name = bodyBone.Name;
            newBody.BoxKind = PEPlugin.Pmd.BodyBoxKind.Box;

            IPXBone fakeBone = bodyBone;

            V3 vecAxis = fakeBone.Parent.Position - fakeBone.Position;

            V3 LocalX = new V3(0, 0, 0);
            V3 LocalZ = new V3(0, 0, 0);
            V3 LocalY = new V3(0, 0, 0);
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
                LocalZ = Vector3.TransformNormal(Vector3.UnitX, matrix);
            }

            vecAxis.Normalize();
            V3 localX = Vector3.Zero;
            V3 localY = Vector3.Zero;
            V3 localZ = Vector3.Zero;
            fakeBone.SetLocalAxis(LocalX, LocalZ);
            fakeBone.IsLocalFrame = true;
            fakeBone.GetLocalAxis(out localX, out localY, out localZ);

            double x = (double)localX.X;
            double y = (double)localX.Y;
            double y2 = (double)localX.Z;
            double x3 = (double)localY.Y;
            double y3 = (double)localZ.X;
            double num = (double)localZ.Y;
            double x4 = (double)localZ.Z;
            double num2 = Math.Atan2(y3, x4);
            double num3 = Math.Asin(-num);
            double num4;
            if (Math.Cos(num3) != 0.0)
            {
                num4 = Math.Atan2(y, x3);
            }
            else
            {
                num2 = Math.Atan2(y2, x);
                num3 = Math.Asin(-num);
                num4 = 0.0;
            }
            float x5 = (float)num3;
            float y4 = (float)num2;
            float z2 = (float)num4;
            V3 rotation = new V3(x5, y4, z2);
            newBody.Rotation = rotation;
        }


    }
}
