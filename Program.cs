using System;
using System.IO.Ports;
using System.Threading;
using OpenHardwareMonitor.Hardware;

namespace Get_CPU_Temp5
{
    class Program
    {
        // Class that visits and updates hardware data
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }

            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware)
                {
                    subHardware.Accept(this);
                }
            }

            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        // Function to get the CPU temperature
        static int GetSystemInfo()
        {
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;  // Enables CPU data collection
            computer.Accept(updateVisitor);

            int cpuTemp = 0;
            bool foundTemp = false;

            // Iterates through the hardware components
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                Console.WriteLine($"Hardware {i}: {computer.Hardware[i].HardwareType}");  // Debugging

                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        Console.WriteLine($"  Sensor {j}: {computer.Hardware[i].Sensors[j].SensorType} - {computer.Hardware[i].Sensors[j].Name}");  // Debugging

                        // Checks if the sensor type is Temperature
                        if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
                        {
                            cpuTemp = (int)computer.Hardware[i].Sensors[j].Value;
                            foundTemp = true;
                            Console.WriteLine("CPU Temperature: " + cpuTemp + "°C");
                        }
                    }
                }
            }

            computer.Close();

            if (foundTemp)
            {
                return cpuTemp;  // Returns the CPU temperature
            }
            else
            {
                return 0;  // Returns 0 if no temperature is found
            }
        }

        static void Main(string[] args)
        {
            // Configures the serial port (with the correct port and baud rate)
            string comPort = "COM6";  // Change to the correct port
            int baudRate = 9600;

            // Initializes serial communication with Arduino
            SerialPort serialPort = new SerialPort(comPort, baudRate);
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening serial port: " + ex.Message);
                return;
            }

            // Waits for the Arduino to prepare for communication
            Thread.Sleep(1000);

            Console.WriteLine("Starting temperature reading...");

            while (true)
            {
                // Gets the CPU temperature
                int cpuTemp = GetSystemInfo();

                // Checks if the temperature was successfully read
                if (cpuTemp > 0)
                {
                    // Sends the temperature to the Arduino via serial
                    serialPort.WriteLine(cpuTemp.ToString());
                    Console.WriteLine("Temperature sent to Arduino: " + cpuTemp + "°C");
                }
                else
                {
                    Console.WriteLine("Unable to read temperature.");
                }

                // Waits 1 second before getting the next temperature
                Thread.Sleep(1000);
            }

            // Closes the serial port when the program ends
            serialPort.Close();
        }
    }
}
