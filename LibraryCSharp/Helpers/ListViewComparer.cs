using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public class ListViewComparer : System.Collections.IComparer
    {
        private int m_iColumnNumber = 0;
        private SortOrder m_SortOrder = SortOrder.Ascending;

        public ListViewComparer(int column_number,
            SortOrder sort_order)
        {
            m_iColumnNumber = column_number;
            m_SortOrder = sort_order;
        }

        // Compare two ListViewItems.
        public int Compare(object object_x, object object_y)
        {
            // Get the objects as ListViewItems.
            ListViewItem item_x = object_x as ListViewItem;
            ListViewItem item_y = object_y as ListViewItem;

            // Get the corresponding sub-item values.
            string string_x;
            if (item_x.SubItems.Count <= m_iColumnNumber)
            {
                string_x = "";
            }
            else
            {
                string_x = item_x.SubItems[m_iColumnNumber].Text;
            }

            string string_y;
            if (item_y.SubItems.Count <= m_iColumnNumber)
            {
                string_y = "";
            }
            else
            {
                string_y = item_y.SubItems[m_iColumnNumber].Text;
            }

            // Compare them.
            int result;
            double double_x, double_y;
            if (double.TryParse(string_x, out double_x) &&
                double.TryParse(string_y, out double_y))
            {
                // Treat as a number.
                result = double_x.CompareTo(double_y);
            }
            else
            {
                DateTime date_x, date_y;
                if (DateTime.TryParse(string_x, out date_x) &&
                    DateTime.TryParse(string_y, out date_y))
                {
                    // Treat as a date.
                    result = date_x.CompareTo(date_y);
                }
                else
                {
                    // Treat as a string.
                    result = string_x.CompareTo(string_y);
                }
            }

            // Return the correct result depending on whether
            // we're sorting ascending or descending.
            if (m_SortOrder == SortOrder.Ascending)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }
}
