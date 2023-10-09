using System.Text;

namespace Parallelization.Model;

public class Matrix
{
    private double[][] _data;

    public Matrix(int size, bool fillIt)
    {
        _data = new double[size][];
        if (fillIt)
        {
            FillWithRandomValues();
        }
        else
        {
            AllocateMemory();
        }
    }

    public int Size { get { return _data.Length; } }

    public double this[int index1, int index2]
    {
        get
        {
            return _data[index1][index2];
        }

        set
        {
            _data[index1][index2] = value;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _data.Length; i++)
        {
            for (int j = 0; j < _data[i].Length; j++)
                sb.Append($"{_data[i][j]}\t");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private void FillWithRandomValues()
    {
        Random rnd = new();
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = new double[_data.Length];
            for (int j = 0; j < _data[i].Length; j++)
                _data[i][j] = rnd.Next(1, 10);
        }
    }

    private void AllocateMemory()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = new double[_data.Length];
        }
    }
}
