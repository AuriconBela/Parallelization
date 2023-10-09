using Parallelization.Model;
using System.Diagnostics;
using System.Text;

namespace Parallelization.UI
{
    public partial class Form1 : Form
    {
        private const int _size = 500;

        private CancellationTokenSource? _cts;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var a = new Matrix(_size, true);
            var b = new Matrix(_size, true);
            var multiplier = new Multiplier(a, b);
            var c = multiplier.Multiply_Raw();

            if (_size < 100)
            {
                var sb = new StringBuilder();
                sb.Append(a).AppendLine().Append(b).AppendLine().Append(c.Data);
                textBox1.Text = sb.ToString();
            }
            else
            {
                MessageBox.Show("Done");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var a = new Matrix(_size, true);
            var b = new Matrix(_size, true);
            var multiplier = new Multiplier(a, b);
            var numberOfThreads = Enumerable.Range(1, 50);
            var times = new List<double>();

            var clock = new Stopwatch();
            foreach (var i in numberOfThreads)
            {
                clock.Restart();
                await Task.Run(() => multiplier.Multiply_Parallel(i));
                clock.Stop();
                times.Add(clock.ElapsedMilliseconds);
            }

            var td = new List<double>();

            foreach (var i in numberOfThreads)
            {
                td.Add(Convert.ToDouble(i));
            }

            formsPlot1.Plot.AddVerticalLine(Environment.ProcessorCount);
            formsPlot1.Plot.AddScatter(td.ToArray(), times.ToArray());
            formsPlot1.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formsPlot1.Plot.XLabel("Number of threads");
            formsPlot1.Plot.YLabel("Run time [ms]");
            formsPlot1.Refresh();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var a = new Matrix(_size, true);
            var b = new Matrix(_size, true);
            var multiplier = new Multiplier(a, b);
            var c = await multiplier.Multiply_Async();

            if (_size < 100)
            {
                var sb = new StringBuilder();
                sb.Append(a).AppendLine().Append(b).AppendLine().Append(c.Data);
                textBox1.Text = sb.ToString();
            }
            else
            {
                MessageBox.Show("Done");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var a = new Matrix(_size, true);
            var b = new Matrix(_size, true);
            var multiplier = new Multiplier(a, b);
            var c = multiplier.Multiply_Parallel();

            if (_size < 100)
            {
                var sb = new StringBuilder();
                sb.Append(a).AppendLine().Append(b).AppendLine().Append(c.Data);
                textBox1.Text = sb.ToString();
            }
            else
            {
                MessageBox.Show("Done");
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            try
            {
                var a = new Matrix(_size, true);
                var b = new Matrix(_size, true);
                var multiplier = new Multiplier(a, b);
                var c = await multiplier.Multiply_Async_AndCancel(_cts);


                switch (c)
                {
                    case Success<Matrix>:
                        if (_size < 100)
                        {
                            var sb = new StringBuilder();
                            sb.Append(a).AppendLine().Append(b).AppendLine().Append(c.Data);
                            textBox1.Text = sb.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Done");
                        }
                        break;
                    case Cancelled<Matrix>:
                        MessageBox.Show("Operation was cancelled");
                        break;
                    case Error<Matrix> err:
                        MessageBox.Show(err.Message);
                        break;
                }
            }
            finally
            {
                _cts.Dispose();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _cts?.Cancel();
        }
    }
}