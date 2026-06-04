using GameCreator.Runtime.Common;

namespace Fullscreen.EditorPro
{
    public class EditorProSettings : AssetRepository<EditorProRepository>
    {
        public override IIcon Icon => new IconEditorPro(ColorTheme.Type.TextNormal);
        public override string Name => "Editor Pro";
    }
}