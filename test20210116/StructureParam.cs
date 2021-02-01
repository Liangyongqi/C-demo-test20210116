namespace test20210116
{
    class StructureParam
    {
        public double diameter;//管径
        public double thickness;//壁厚
        public double standardPressure;//标准承压
        public double price;//价格
        public double strength;//屈服强度

        public StructureParam(double diameter, double thickness, double standardPressure, double price, double strength)
        {
            this.diameter = diameter;
            this.thickness = thickness;
            this.standardPressure = standardPressure;
            this.price = price;
            this.strength = strength;
        }


    }
}
