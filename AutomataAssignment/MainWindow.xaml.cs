using Microsoft.Msagl.Core.Layout;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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


namespace AutomataAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private OpenFileDialog openFileDialog1;
        private List<State> States = new List<State>();
        private int NumberOfTransactions;
        private List<Transaction> Transactions = new List<Transaction>();
        private State StartState;


        System.Windows.Forms.Form form = new System.Windows.Forms.Form();
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");


        public MainWindow()
        {
            var appLoaction = AppDomain.CurrentDomain.BaseDirectory;
            var appPath = System.IO.Path.Combine(appLoaction,"logs\\appLog.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(appPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Debug("starts!!");
            InitializeComponent();
        }

        private void deterministicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReadFile(".da");
            form = new System.Windows.Forms.Form();
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            graph = new Microsoft.Msagl.Drawing.Graph("graph");
            DrawGraph();
        }

        private void nondeterministicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReadFile(".nda");
        }

        private void nondeterministicWithEMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ReadFile(".ndae");
        }

        private void ReadFile(string v)
        {
            States = new List<State>();
            Transactions = new List<Transaction>();
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = $"da files (*{v})|*{v}|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog1.FileName);
                    var str = sr.ReadToEnd();
                    sr.Dispose();
                    string[] lines = str.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                    foreach (var item in lines)
                    {
                        if (item.Length != 0)
                        {
                            var index = Array.IndexOf(lines, item);
                            switch (index)
                            {
                                case 0:
                                    var statenum = int.Parse(item);
                                    for (int i = 0; i < statenum; i++)
                                    {

                                        States.Add(new State
                                        {
                                            Final = false,
                                            Id = i + 1,
                                            Name = (i + 1).ToString(),
                                            Start = false
                                        });

                                    }
                                    foreach (var state in States)
                                    {
                                        Log.Debug(state.ToString());
                                    }
                                    break;
                                case 1:
                                    var firstState = int.Parse(item);
                                    var fState = States.Where(x => x.Id == firstState).FirstOrDefault();
                                    fState.Start = true;
                                    Log.Debug("First state" + fState.ToString());
                                    break;
                                case 3:
                                    foreach (var a in item)
                                    {
                                        if (a != ' ')
                                        {
                                            var laststate = int.Parse(a.ToString());
                                            var state = States.Where(x => x.Id == laststate).FirstOrDefault();
                                            state.Final = true;
                                            Log.Debug("last state" + state.ToString());

                                        }
                                    }
                                    break;
                                case 4:
                                    NumberOfTransactions = int.Parse(item);
                                    Log.Debug("number of transactions " + item);
                                    break;

                            }
                            if (index == 4)
                            {
                                break;
                            }
                        }

                    }
                    for (var i = 5; i < NumberOfTransactions + 5; i++)
                    {
                        var s = lines[i];
                        var a = new Transaction
                        {
                            StartingPoint = States[int.Parse(s[0].ToString()) - 1],
                            CharInserted = s[2].ToString(),
                            EndingPoint = States[int.Parse(s[4].ToString()) - 1]

                        };
                        Transactions.Add(a);
                        Log.Debug("Transaction made: " + a.ToString());



                    }
                    foreach (var state in States)
                    {
                        state.AbleTransactions = Transactions.Where(x => x.StartingPoint.Id == state.Id).ToList();
                        state.TransactionCameFrom = Transactions.Where(x => x.EndingPoint.Id == state.Id).ToList();
                    }
                    Log.Debug("File is read");
                    foreach (var s in States)
                    {
                        Log.Debug(s.ToString());
                    }
                    foreach (var a in Transactions)
                    {
                        Log.Debug(a.ToString());
                    }
                    checkButton.IsEnabled = true;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }

        }

        private void DrawGraph()
        {
            foreach(var trans in Transactions)
            {
                graph.AddEdge(trans.StartingPoint.Name, trans.CharInserted, trans.EndingPoint.Name);
            }

            StartState = States.Where(x => x.Start == true).FirstOrDefault();
            var startnode = graph.FindNode(StartState.Name);
            startnode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.InvHouse;

            var endstates = States.Where(x => x.Final == true).ToList();
            var endnodes = new List<Microsoft.Msagl.Drawing.Node>();
            foreach(State s in endstates)
            {
                var a = graph.FindNode(s.Name);
                a.Attr.Shape = Microsoft.Msagl.Drawing.Shape.DoubleCircle;
                endnodes.Add(a);
            }
            

            viewer.Graph = graph;
            viewer.Size = new System.Drawing.Size(500, 500);
            viewer.ToolBarIsVisible = false;
            form.Size = new System.Drawing.Size(500, 500);
            form.SuspendLayout();

            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            
            form.Show();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var Word = BaseTextBox.Text;
            var flag = false;


            var CurrentState = new List<State> { StartState };
            var Destinations = new List<State>();
            foreach (var l in Word)
            {
                var letter = l.ToString();
                
                foreach(var state in CurrentState)
                {
                    Destinations.AddRange(state.Resolve(letter));
                }
                CurrentState = new List<State>(Destinations);
                Destinations = new List<State>();

            }
            foreach(var state in CurrentState)
            {
                if (state.Final)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                theword.Text = "true";
            }
            else
            {
                theword.Text = "false";
            }
            CurrentState = new List<State>();
            Destinations = new List<State>();

        }

    }
}
