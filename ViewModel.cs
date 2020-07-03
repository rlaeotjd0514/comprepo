using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
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
    public interface INode
    {
        string Name { get; set; } //보여질 이름
        string Path { get; set; } //내부적으로 가지고 있을 전체 경로
        object Etc { get; set; } //string[] : 세부적으로 사용하는 정보 (DataGridView에 전달할 DEFECT 정보 리스트 등)
    }

    public class SC_Data : INode, INotifyPropertyChanged
    {        
        public SC_Data(SC_INFO sinfo)
        {
            this.SC_Info = sinfo;
            this.Children = new ObservableCollection<INode>();
        }

        public SC_Data(SC_INFO sinfo, SC_Data parent) : this(sinfo)
        {
            this.Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if(handler == null)
            {
                return;
            }
            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BuildSCInformation()
        {
            
        }

        private ObservableCollection<INode> children;
        public ObservableCollection<INode> Children
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
                this.NotifyPropertyChanged();
            }
        }

        public SC_Data Parent { get; set; }
        public SC_INFO SC_Info { get; set; }        

        public class SC_INFO
        {
            public object information { get; set; }
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public object Etc { get; set; }
    }

    public class EmptyNode : INode
    {
        public EmptyNode(SC_Data parent)
        {
            this.Parent = parent;
        }

        public string Name { get; set; }
        public string Path
        {
            get
            {
                return this.Parent == null ? string.Empty : this.Parent.Path;
            }
            set
            {
                this.Parent.Path = value;
            }
        }

        public object Etc { get; set; }
        public SC_Data Parent { get; set; }
    }    

    public class SCINFO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private TreeViewItem h;

        public TreeViewItem H
        {
            get
            {
                return h;   
            }
            set
            {
                h = value;
                this.NotifyPropertyChanged();
            }
        }
    }

    public class ViewModel : INotifyPropertyChanged, ICommand
    {
        #region COMMAND
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {

        }
        #endregion

        DataTable PANEL;
        DataTable SUBPANEL;
        DataTable DEFECT;
        
        private List<string[]> datalist;
        private int line_num;

        private TreeViewItem headTVI;
        public TreeViewItem HeadTVI
        {
            get
            {
                return headTVI;
            }
            set
            {
                headTVI = value;
                NotifyPropertyChanged("HeadTVI");
            }
        }
        
        private string datafilepath = @"C:\Users\dskim\Desktop\QD_SampleFile\BP IL AOI Photo.txt";
        public string DataFilePath
        {
            get
            {
                return this.datafilepath;                
            }
            set
            {
                this.datafilepath = value;
                NotifyPropertyChanged("DataFilePath");
            }
        }        
        private INode selectednode;
        public INode SelectedNode
        {
            get
            {
                return this.selectednode;
            }
            set
            {
                this.selectednode = value;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        enum Types { ERROR = 0, PANEL = 1, SUBPANEL = 2, DEFECT = 3 };
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

        public ViewModel()
        {
            if (!File.Exists(this.datafilepath))
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PANEL = new DataTable();
            SUBPANEL = new DataTable();
            DEFECT = new DataTable();
            datalist = new List<string[]>();

            FileStream fs = new FileStream(this.datafilepath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            for (int i = 1; i <= 3; i++)
            {
                string line = sr.ReadLine();
                string[] args = line.Split(' ');
                int nu_count = 1;
                for (int j = 0; j <= args.Length - 1; j++)
                {
                    if (args[1] == "PANEL")
                    {
                        if (args[j] == "NO_USE")
                        {
                            PANEL.Columns.Add(args[j] + nu_count, typeof(string));
                            nu_count++;
                        }
                        else
                        {
                            PANEL.Columns.Add(args[j], typeof(string));
                        }
                    }
                    else if (args[1] == "SUBPANEL")
                    {
                        if (args[j] == "NO_USE")
                        {
                            SUBPANEL.Columns.Add(args[j] + nu_count, typeof(string));
                            nu_count++;
                        }
                        else
                        {
                            SUBPANEL.Columns.Add(args[j], typeof(string));
                        }
                    }
                    else
                    {
                        if (args[j] == "NO_USE")
                        {
                            DEFECT.Columns.Add(args[j] + nu_count, typeof(string));
                            nu_count++;
                        }
                        else
                        {
                            DEFECT.Columns.Add(args[j], typeof(string));
                        }
                    }
                }
            }

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] args = line.Split(' ');

                List<string> anealed = new List<string>();
                if (args.Length == 0) continue;
                foreach (var c in args)
                {
                    if (c != "")
                    {
                        anealed.Add(c);
                    }
                }
                datalist.Add(anealed.ToArray());
            }
            line_num = datalist.Count;

            var HeadNode = CreateLists(0, Types.ERROR);
            HeadNode.Header = "Head Node";

            HeadTVI = HeadNode;

            sr.Close();
            fs.Close();
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