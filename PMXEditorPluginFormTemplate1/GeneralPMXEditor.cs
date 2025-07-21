using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEPExtensions;
using PEPlugin;
using PEPlugin.Pmx;
using PEPlugin.SDX;
using PEPlugin.Pmd;
using PEPlugin.Vmd;
using PEPlugin.Vme;
using PEPlugin.Form;
using PEPlugin.View;
using PXCPlugin;
using Newtonsoft.Json;


namespace ManaLazyTool
{
    public class GeneralPMXEditor
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

        public void GetCurrentInfo()
        {
            pmx = connect.Pmx.GetCurrentState();
            bone = pmx.Bone;
            vertex = pmx.Vertex;
            header = pmx.Header;
            material = pmx.Material;
            info = pmx.ModelInfo;
        }

        //public void GetCurrentInfo(object sender, EventArgs e)
        //{
        //    pmx = connect.Pmx.GetCurrentState();
        //    bone = pmx.Bone;
        //    vertex = pmx.Vertex;
        //    header = pmx.Header;
        //    material = pmx.Material;
        //    info = pmx.ModelInfo;
        //}

        public void UpdateCurrentInfo()
        {
            connect.Pmx.Update(pmx); // update the info in pmx
            connect.Form.UpdateList(UpdateObject.All); // update form
            connect.View.PMDView.UpdateModel(); // refresh model ot reflect new changes
            connect.View.PMDView.UpdateView(); // refresh view to reflect new changes
        }

        public void UpdateCurrentInfo(object sender, EventArgs e)
        {
            connect.Pmx.Update(pmx); // update the info in pmx
            connect.Form.UpdateList(UpdateObject.All); // update form
            connect.View.PMDView.UpdateModel(); // refresh model ot reflect new changes
            connect.View.PMDView.UpdateView(); // refresh view to reflect new changes
        }

    }
}
