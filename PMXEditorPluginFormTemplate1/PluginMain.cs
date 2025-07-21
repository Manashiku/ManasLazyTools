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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManaLazyTool
{
    public class PMXEditorPluginFormTemplate1 : IPEPlugin
    {
        private bool disposedValue;

        public string Name => "Mana's Lazy Tools";

        public string Version => "1.0";

        public string Description => "General helper tools";

        public IPEPluginOption Option => new PEPluginOption(false, true);

        private FormControl form;

        public void Run(IPERunArgs args)
        {
            try
            {
                if (form == null)
                {
                    form = new FormControl(args);
                    form.Visible = true;
                    form.Reload();
                }
                else
                {
                    form.Visible = !form.Visible;
                    if (form.Visible)
                        form.Reload();
                }
            }
            catch (Exception ex)
            {
                Utility.ShowException(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~Class1()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
