using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEPlugin;
using SlimDX;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using PEPlugin.View;

namespace ManaLazyTool
{
    class ShaderObjects
    {
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
        public IPXHeader header;
        public IPXModelInfo info;
        public IPXModelInfo modelInfo;

        public void DefaultShaderObjects()
        {

        }

        public void ShinyColorShaderObjects(IList<IPXMaterial> materialList, IPXPmxBuilder builder, IList<IPXBone> boneList)
        {
            PaintShadow(materialList);
            IPXBone angelRingOffsetter = builder.Bone();
            boneList.Add(angelRingOffsetter);
            angelRingOffsetter.Name = "ANGEL_RING_OFFSET";
            angelRingOffsetter.IsTranslation = true;
            foreach(IPXBone bones in boneList)
            {
                if(bones.Name == "頭")
                {
                    angelRingOffsetter.Parent = bones;
                    angelRingOffsetter.Position = bones.Position;
                }
            }

        }

        public void DuplicateMatrialsAndFaces()
        {

        }

        private void PaintShadow(IList<IPXMaterial> materialList)
        {
            foreach (IPXMaterial mat in materialList)
            {

                foreach (IPXFace fce in mat.Faces)
                {
                    PaintUV3VertexPos(fce.Vertex1);
                    PaintUV3VertexPos(fce.Vertex2);
                    PaintUV3VertexPos(fce.Vertex3);
                }
            }
        }

        private void PaintUV3VertexPos(IPXVertex vertex)
        {
            Vector3 offsetPos = vertex.Position;
            vertex.UVA3 = new Vector4(vertex.Position, vertex.UVA3.W);
        }
    
       
    }
}
