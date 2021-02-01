using System.Collections.Generic;

namespace test20210116
{
    class MTree
    {
        /// <summary>
        /// 声明全局变量
        /// </summary>
        List<node> listNodes = new List<node>();
        /// <summary>
        /// 遍历单个节点
        /// </summary>
        /// <param name="TreeView"></param>
        public void TraverseNode(node TreeView)
        {
            node Children = new node();
            Children = TreeView.Clone() as node;
            Children.nodes = null;
            listNodes.Add(Children);
        }
        /// <summary>
        /// 递归遍历树
        /// </summary>
        /// <param name="TreeView"></param>
        public void GetSplitList(node TreeView)
        {
            TraverseNode(TreeView);
            if (TreeView.nodes != null)
            {
                for (int i = 0; i < TreeView.nodes.Count; i++)
                {
                    GetSplitList(TreeView.nodes[i]);
                }
            }
        }
    }
    public class node
    {
        public string text { get; set; }
        public string tags { get; set; }
        public string id { get; set; }
        public string parentId { get; set; }
        public string color { get; set; }
        public List<node> nodes { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
