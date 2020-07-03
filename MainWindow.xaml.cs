using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data;

namespace defect_lister_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitDataField();
        }

        void InitDataField()
        {
            PANEL = new DataTable();
            SUBPANEL = new DataTable();
            DEFECT = new DataTable();
        }

        enum Types { ERROR = 0, PANEL = 1, SUBPANEL = 2, DEFECT = 3 };
        string datafilePath = "";
        DataTable PANEL;
        DataTable SUBPANEL;
        DataTable DEFECT;
        int line_num = 0;

        Types tres(string s)
        {
            if (s == "PANEL")
            {
                return Types.PANEL;
            }
            if (s == "SUBPANEL")
            {
                return Types.SUBPANEL;
            }
            if (s == "DEFECT")
            {
                return Types.DEFECT;
            }
            return Types.ERROR;
        }

        void ClearData()
        {
            PANEL.Columns.Clear();
            PANEL.Rows.Clear();
            SUBPANEL.Columns.Clear();
            SUBPANEL.Rows.Clear();
            DEFECT.Columns.Clear();
            DEFECT.Rows.Clear();
        }        

        //open button click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.ShowDialog();
            FilePath.Text = ofd.FileName;
            datafilePath = ofd.FileName;
        }

        //parse button click
        List<string[]> datalist = new List<string[]>();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {            
        }        

        TreeViewItem CreateLists(int index, Types e)
        {
            TreeViewItem sublist = new TreeViewItem();
            int si = index;
            while (si < line_num)
            {
                Types current = tres(datalist[si][1]);
                if (e >= current) break;
                if (current != e + 1)
                {
                    si += 1;
                    continue;
                }
                if (current == Types.DEFECT)
                {
                    var t = new TreeViewItem();
                    t.Header = datalist[si][2] + ":NO." + datalist[si][3];
                    t.Tag = datalist[si];
                    sublist.Items.Add(t);
                    DEFECT.Rows.Add(datalist[si]);
                    si += 1;
                    continue;
                }

                sublist.Tag = datalist[si];

                if (e < current)
                {
                    var res = CreateLists(si + 1, current);
                    if (current == Types.PANEL)
                    {
                        res.Header = datalist[si][3];
                        res.Tag = datalist[si];
                        PANEL.Rows.Add(datalist[si]);
                    }
                    else if (current == Types.SUBPANEL)
                    {
                        res.Header = datalist[si][2];
                        res.Tag = datalist[si];
                        SUBPANEL.Rows.Add(datalist[si]);
                    }
                    sublist.Items.Add(res);
                    si += res.Items.Count + 1;
                }
            }
            return sublist;
        }
    }
}
