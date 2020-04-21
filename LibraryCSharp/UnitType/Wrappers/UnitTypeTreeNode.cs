using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Library.UnitType.Wrappers
{
	public class UnitTypeTreeNode
	{
		private UnitTypeTreeViewManager m_TreeViewManager = null;
		private UnitTypeManager m_UnitTypeManager = null;				
		private string m_strUnitTypeCodeName = "";
		private int m_iLastValidID = 0;
		private TreeNode m_TreeNodeAndChildren = new TreeNode();		
		public UnitTypeTreeNode(UnitTypeTreeViewManager mTreeViewManager, UnitType mUnitType)
		{
			if (mUnitType != null)
			{
				m_strUnitTypeCodeName = mUnitType.unitTypeName;
			}
			m_TreeViewManager = mTreeViewManager;
			if( m_TreeViewManager != null)
			{
				m_UnitTypeManager = m_TreeViewManager.getUnitTypeManager();				
			}
			m_TreeNodeAndChildren.NodeFont = new Font(m_TreeViewManager.getTreeView().Font, FontStyle.Bold);
			
			refreshTreeNodes();
		}

		public UnitTypeCategory getCategory() { return m_TreeViewManager.getUnitTypeCategory(); }

		public int getLastUnitTypeValidID() { return m_iLastValidID;  }

		public bool getNeedsToBeDeleted() 
		{
			UnitType mUnitType = getUnitType();
			if( mUnitType == null)
			{
				m_TreeNodeAndChildren.Remove();
				return true;
			}
			return false;
		}

		public TreeNode getTreeNode()
		{
			return m_TreeNodeAndChildren;
		}

		public UnitType getUnitType() 
		{ 
			if(getCategory() != null)
			{
				return getCategory().getUnitTypeByCodeName(m_strUnitTypeCodeName);
			}
			return null; 
		}

		//attempts to find out if it needs to be added or removed from the tree based on the filter
		public void filterTreeNode()
		{
			if( getNeedsToBeDeleted() )
			{
				m_TreeNodeAndChildren.Remove();
				return;
			}
			string strFilterString = m_TreeViewManager.unitTypeFilterString;
			string strFilterByUnitType = m_TreeViewManager.unitTypeFilterByUnitType;
			//does our unit type name contain the filter string?
			if(strFilterString != "" &&
				m_strUnitTypeCodeName.Contains(strFilterString) == false )
			{
				m_TreeNodeAndChildren.Remove();
				return;
			}
			//if there is no unit type just add us back in!
			if( strFilterByUnitType == "" )
			{
				if(m_TreeNodeAndChildren.TreeView == null )
				{
					m_TreeViewManager.getTreeView().Nodes.Add(m_TreeNodeAndChildren);
				}
				return;
			}
			//check the unit type
			UnitType mUnitType = getUnitType();
			if( mUnitType == null ||
				mUnitType.isa(strFilterByUnitType) == false )
			{
				m_TreeNodeAndChildren.Remove();
				return;
			}
			//finally we made it here attempt to add it back in
			if (m_TreeNodeAndChildren.TreeView == null)
			{
				m_TreeViewManager.getTreeView().Nodes.Add(m_TreeNodeAndChildren);
			}

		}

		public void refreshTreeNodes()
		{
			UnitType mUnitType = getUnitType();
			if(mUnitType == null)
			{
				return;
			}
			if (m_iLastValidID != mUnitType.getHashCodeOfUnitType())
			{
				m_iLastValidID = mUnitType.getHashCodeOfUnitType();
				m_TreeNodeAndChildren.Text = mUnitType.unitTypeName;
				//NOTE to self - this is excessive. However creating and adding children is super expensive.
				//so we have to minimize the times we do it. So we try as hard as we can to only remove and add
				//the tree nodes we HAVE to.
				_removeChildren(m_TreeNodeAndChildren, mUnitType.fullHierarchyOfChildren);
				_addChildren(m_TreeNodeAndChildren, mUnitType, 0);
			}
			filterTreeNode();
		}

		//adds children recursively
		private void _addChildren(TreeNode mParentTreeNode, UnitType mUnitType, int iDepth)
		{
			foreach(string strChildName in mUnitType.immediateChildren)
			{
				UnitType mChildUnitType = getCategory().getUnitTypeByCodeName(strChildName);
				if( mChildUnitType == null)
				{
					//probably should show an error here
					continue;
				}
				TreeNode mChildTreeNodeFound = null;
				foreach(TreeNode mChildTreeNode in mParentTreeNode.Nodes)
				{
					if(mChildTreeNode.Text == mChildUnitType.unitTypeName)
					{
						mChildTreeNodeFound = mChildTreeNode;
						break;
					}
				}
				if(mChildTreeNodeFound == null )	//not found
				{
					//we have to add it on!
					mChildTreeNodeFound = mParentTreeNode.Nodes.Add(mChildUnitType.unitTypeName);
					if (iDepth == 0)
					{
						mChildTreeNodeFound.NodeFont = new Font(m_TreeViewManager.getTreeView().Font.Name, m_TreeViewManager.getTreeView().Font.Size, FontStyle.Regular);
					}
					else
					{
						mChildTreeNodeFound.NodeFont = new Font(m_TreeViewManager.getTreeView().Font.Name, m_TreeViewManager.getTreeView().Font.Size - 1, FontStyle.Italic);
					}

				}				
				_addChildren(mChildTreeNodeFound, mChildUnitType, (iDepth + 1));	//add any children here.
			}

		}

		//removes children recursively
		private void _removeChildren(TreeNode mParentTreeNode, List<String> mChildrenByCodeNames)
		{
			for(int iChildIndex = 0; iChildIndex < mParentTreeNode.Nodes.Count; iChildIndex++ ) 
			{
				if (mChildrenByCodeNames.Contains(mParentTreeNode.Nodes[iChildIndex].Text) == false )
				{
					mParentTreeNode.Nodes.RemoveAt(iChildIndex);
					iChildIndex++;
				}
				else
				{
					_removeChildren(mParentTreeNode.Nodes[iChildIndex], mChildrenByCodeNames);
				}
			}
		}
	}
}
