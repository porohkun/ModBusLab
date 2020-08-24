using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
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

namespace ModBusMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private Register[] _registers = new Register[10]
        {
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0),
            new Register(0)
        };

        public Register[] Registers
        {
            get => _registers;
            set
            {
                if (_registers != value)
                {
                    _registers = value;
                    NotifyPropertyChanged(nameof(Registers));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ReadRegisters_Click(object sender, RoutedEventArgs e)
        {
            using (var port = GetSerialPort())
            {
                var factory = new ModbusFactory();
                IModbusSerialMaster master = factory.CreateRtuMaster(port);

                byte slaveId = 1;
                ushort startAddress = 0;
                ushort numRegisters = 10;

                // read five registers		
                Registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters).Select(r => new Register(r)).ToArray();
            }
        }

        private void WriteRegisters_Click(object sender, RoutedEventArgs e)
        {
            using (var port = GetSerialPort())
            {
                var factory = new ModbusFactory();
                IModbusSerialMaster master = factory.CreateRtuMaster(port);

                byte slaveId = 1;
                ushort startAddress = 0;

                master.WriteMultipleRegisters(slaveId, startAddress, Registers.Select(r => r.Value).ToArray());
            }
        }

        private SerialPort GetSerialPort()
        {
            SerialPort port = new SerialPort("COM4")
            {
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            port.Open();
            return port;
        }
    }

    public class Register
    {
        public ushort Value { get; set; }

        public Register(ushort value)
        {
            Value = value;
        }
    }
}
