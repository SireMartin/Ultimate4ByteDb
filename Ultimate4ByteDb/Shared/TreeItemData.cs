using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultimate4ByteDb.Shared
{
    public class TreeItemData
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public bool IsExpanded { get; set; }
        public int? Occurence { get; set; }
        public float? Likelyhood { get; set; }
        public bool HasChild => TreeItems != null && TreeItems.Count > 0;
        public HashSet<TreeItemData> TreeItems { get; set; } = new HashSet<TreeItemData>();

        public TreeItemData(string text, string icon)
        {
            Text = text;
            Icon = icon;
        }
    }
}
