using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace 外骨骼界面程序
{
    public class SensorDataMode
    {
        private List<SensorData> SensorDataList = new List<SensorData>();

        [XmlElement(ElementName = "SensorData")]
        public List<SensorData> SensorDatas
        {
            get { return SensorDataList; }
            set { SensorDataList = value; }
        }

    }

    public class SensorData
    {
        private List<Motor> motors = new List<Motor>();
        private List<Dip> dips = new List<Dip>();
        private PressureSender pressures = new PressureSender();
        private AngleSender angles = new AngleSender();
        public SensorData()
        { }

        private string timeString;
        public SensorData(string timeString)
        {
            this.timeString = timeString;

        }
        public string TimeString
        {
            get
            {
                return timeString;
            }

            set
            {
                timeString = value;
            }
        }
        [XmlElement(ElementName = "Motor")]
        public List<Motor> Motors
        {
            get
            {
                return motors;
            }

            set
            {
                motors = value;
            }
        }


        [XmlElement(ElementName = "pressure")]
        public PressureSender Pressures
        {
            get
            {
                return pressures;
            }

            set
            {
                pressures = value;
            }
        }
        [XmlElement(ElementName = "angle")]
        public AngleSender Angles
        {
            get
            {
                return angles;
            }

            set
            {
                angles = value;
            }
        }

        [XmlElement(ElementName = "Dip")]
        public List<Dip> Dips
        {
            get
            {
                return dips;
            }

            set
            {
                dips = value;
            }
        }




    }

    public class AngleSender
    {
        private double angle;
        private double angle1;
        private double angle2;
        private double angle3;
        public AngleSender() { }

        public AngleSender(double angle, double angle1, double angle2, double angle3)
        {
            this.angle = angle;
            this.angle1 = angle1;
            this.angle2 = angle2;
            this.angle3 = angle3;
        }

        public double Angle
        {
            get
            {
                return angle;
            }

            set
            {
                angle = value;
            }
        }

        public double Angle1
        {
            get
            {
                return angle1;
            }

            set
            {
                angle1 = value;
            }
        }

        public double Angle2
        {
            get
            {
                return angle2;
            }

            set
            {
                angle2 = value;
            }
        }

        public double Angle3
        {
            get
            {
                return angle3;
            }

            set
            {
                angle3 = value;
            }
        }
    }

    public class PressureSender
    {
        private int pressure;
        private int pressure1;
        private int pressure2;
        private int pressure3;
        private int pressure4;
        private int pressure5;
        private int pressure6;
        private int pressure7;

        public PressureSender() { }

        public PressureSender(int pressure, int pressure1, int pressure2, int pressure3, int pressure4, int pressure5, int pressure6, int pressure7)
        {
            this.pressure = pressure;
            this.pressure1 = pressure1;
            this.pressure2 = pressure2;
            this.pressure3 = pressure3;
            this.pressure4 = pressure4;
            this.pressure5 = pressure5;
            this.pressure6 = pressure6;
            this.pressure7 = pressure7;
        }

        public int Pressure
        {
            get
            {
                return pressure;
            }

            set
            {
                pressure = value;
            }
        }

        public int Pressure1
        {
            get
            {
                return pressure1;
            }

            set
            {
                pressure1 = value;
            }
        }

        public int Pressure2
        {
            get
            {
                return pressure2;
            }

            set
            {
                pressure2 = value;
            }
        }

        public int Pressure3
        {
            get
            {
                return pressure3;
            }

            set
            {
                pressure3 = value;
            }
        }

        public int Pressure4
        {
            get
            {
                return pressure4;
            }

            set
            {
                pressure4 = value;
            }
        }

        public int Pressure5
        {
            get
            {
                return pressure5;
            }

            set
            {
                pressure5 = value;
            }
        }

        public int Pressure6
        {
            get
            {
                return pressure6;
            }

            set
            {
                pressure6 = value;
            }
        }

        public int Pressure7
        {
            get
            {
                return pressure7;
            }

            set
            {
                pressure7 = value;
            }
        }
    }

    public class Dip
    {
        public Dip()
        { }

        private double ba;
        private double lr;

        public Dip(double ba, double lr)
        {
            this.ba = ba;
            this.lr = lr;
        }
        public double Ba
        {
            get
            {
                return ba;
            }

            set
            {
                ba = value;
            }
        }

        public double Lr
        {
            get
            {
                return lr;
            }

            set
            {
                lr = value;
            }
        }
    }

    public class Motor
    {
        int enable;
        int direction;
        double rotatingSpeed;
        double motorangle;

        public Motor()
        { }

        public Motor(int enable, int direction, double rotatingSpeed, double motorangle)
        {
            this.enable = enable;
            this.direction = direction;
            this.rotatingSpeed = rotatingSpeed;
            this.motorangle = motorangle;
        }
        public int Enable
        {
            get
            {
                return enable;
            }

            set
            {
                enable = value;
            }
        }

        public int Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        public double RotatingSpeed
        {
            get
            {
                return rotatingSpeed;
            }

            set
            {
                rotatingSpeed = value;
            }
        }

        public double MotorAngle
        {
            get
            {
                return motorangle;
            }

            set
            {
                motorangle = value;
            }
        }
    }


}
