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
            readFile(".da");
        }

        private void nondeterministicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            readFile(".nda");
        }

        private void nondeterministicWithEMenuItem_Click(object sender, RoutedEventArgs e)
        {
            readFile(".ndae");
        }

        private void readFile(string v)
        {
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
                    Log.Debug("File is read");
                    foreach (var s in States)
                    {
                        Log.Debug(s.ToString());
                    }
                    foreach (var a in Transactions)
                    {
                        Log.Debug(a.ToString());
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }

        }
    }
}
