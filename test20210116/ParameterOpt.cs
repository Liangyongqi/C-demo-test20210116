using System;
using System.Collections.Generic;

namespace test20210116
{
    class ParameterOpt
    {
        const int N = 9;//站点数量
        string[] stationNameArray = new string[N] { "194", "195", "196", "197", "198", "199", "200", "201", "脱水站", };
        //int[] wellNum = new int[N] { 3, 5, 1, 6, 3, 3, 2, 3, 0 };//井式
        int[] wellNum = new int[N] { 4, 6, 4, 6, 6, 3, 2, 4, 0 };//井式
        double[] qDesignArray = new Double[N];//设计输气量
        double[] qCalculateArray = new Double[N];//计算输气量
        double[] qleijiaArray = new Double[N];//累加输气量
        double p1 = 0.0;//起点压力值
        double q1 = 0.0;//起点产量
        double p2 = 4.5;//终点压力值，默认是脱水站的压力
        double final_price = 0;
        double[] pArray = new Double[N] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] pipeArray = new int[N] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//存放管道参数的索引
        double[] priceArray = new Double[N] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//存放各管段的总费用；


        //站点之间的距离矩阵194-201,脱水站
        double[,] distance = new Double[N, N] {
                { 0, 1234.99, 0, 0, 0, 0, 0, 0, 0 },//194
                { 0, 0, 0, 0, 0, 1313.35, 0, 0, 0 }, //195
                { 0, 0, 0, 0, 0, 1422.97, 0, 0, 0 },//196
                { 737.64, 0, 0, 0, 0, 0, 0, 0, 0 }, //197
                { 0, 0, 0, 0, 0, 0, 577.74, 0, 0 }, //198
                { 0, 0, 0, 0, 0, 0, 890.1, 0, 0 }, //199
                { 0, 0, 0, 0, 0, 0, 0, 0, 1604.97 },//200
                { 0, 0, 0, 0, 0, 0, 0, 0, 2481.5 },//201
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 } //脱水站
            };
        //管径壁厚组合
        //StructureParam structureParam = new StructureParam() ;
        List<StructureParam> structureParamList = new List<StructureParam>();



        public void optimization()
        {
            //存储管径壁厚组合(外径，壁厚，压力，价格)
            //structureParamList.Add(new StructureParam(323.9, 9,6.3, 8067.241));
            //structureParamList.Add(new StructureParam(219, 7, 6.3, 7953.448));
            //structureParamList.Add(new StructureParam(168, 6.5, 6.3, 8345.584));
            //structureParamList.Add(new StructureParam(76, 12, 6.3, 7941.372));

            //structureParamList.Add(new StructureParam(76, 12, 42, 8067.241, 360));
            //structureParamList.Add(new StructureParam(76, 4, 42, 7953.448, 360));
            //structureParamList.Add(new StructureParam(89, 5, 42, 8345.584, 360));
            structureParamList.Add(new StructureParam(114.3, 5, 6.3, 7941.372, 360));
            structureParamList.Add(new StructureParam(168, 6.5, 6.3, 8067.241, 360));
            structureParamList.Add(new StructureParam(168.3, 7.11, 6.3, 7953.448, 360));
            //structureParamList.Add(new StructureParam(219, 7, 6.3, 8345.584, 245));
            structureParamList.Add(new StructureParam(219, 7, 6.3, 7941.372, 360));
            structureParamList.Add(new StructureParam(323.9, 9, 6.3, 8067.241, 360));
            structureParamList.Add(new StructureParam(323.9, 10, 6.3, 7953.448, 360));

            structureParamList.Add(new StructureParam(323.9, 8, 6.3, 6800, 360));
            structureParamList.Add(new StructureParam(168, 6.5, 6.3, 6600, 360));
            //structureParamList.Add(new StructureParam(76, 10, 42, 6400, 360));



            //设计输气量为6.5*104m3/a,计算输气量为设计输气量的1.3倍
            for (int i = 0; i < N; i++)
            {
                qDesignArray[i] = wellNum[i] * 6.5;
                qCalculateArray[i] = qDesignArray[i] * 1.3;
            }

            //如果距离小于600，则看做采气平台，第二个站点的产量数据加上上一个点的产量。采气管线集输半径在0.75-1.3km之间,取直线距离1.3km。
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0; j < N; j++)
            //    {
            //        if (distance[i,j] != 0)
            //        {
            //            qleijiaArray[j] = qleijiaArray[j] ;
            //        }


            //    }
            //}

            //获取所有节点的累加流量
            for (int i = 0; i < N; i++)
            {
                qleijiaArray[i] = getUpYield(i);

            }
            //计算管道承压能力
            foreach (var item in structureParamList)
            {
                item.standardPressure = (item.thickness - 1) * (2 * 360 * 1 * 0.5 * 1) / item.diameter;
            }

            //获取所有符合条件的管道参数。
            getPipeParam(N-1, p2);//计算压力公式

            for (int i = 0; i < N - 1; i++)
            {
                Console.WriteLine(i + "： " + stationNameArray[i] + "的管径和壁厚： " + structureParamList[pipeArray[i]].diameter + " " + structureParamList[pipeArray[i]].thickness + " 费用： "+priceArray[i]);
                final_price += priceArray[i];
            }
            Console.WriteLine("总费用： " + final_price);

            //Console.WriteLine("李智慧公式可研干线压力值： " + getP1(4.5, 273, 4800, 300));
            //Console.WriteLine("李智慧公式可研干线压力值： " + getP1(4.5, 195, 4800, 300));
            //Console.WriteLine("李智慧公式可研干线压力值： " + getP1(4.5, 273, 4800, 168));
            //Console.WriteLine("李智慧公式可研干线压力值： " + getP1(4.5, 261.95, 4800, 168));
            //Console.WriteLine("323.9真实承压能力： " + (8 - 1) * (2 * 360 * 1 * 0.5 * 1) / 323.9);
            //Console.WriteLine("168真实承压能力： " + (6.5 - 1) * (2 * 360 * 1 * 0.5 * 1) / 168);

        }
        //递归获取某个节点的流量。
        public double getUpYield(int j)
        {
            double result = 0.0;
            int flag = 0;
            for (int i = 0; i < N; i++)
            {

                if (distance[i, j] != 0)
                {
                    result = result + getUpYield(i);
                    flag = 1;
                }
            }

            if (flag == 0)
            {
                return qCalculateArray[j];
            }
            else
            {
                return result + qCalculateArray[j];
            }
        }
        //递归获取某个节点的压力。输入脱水站的索引值
        public void getPipeParam(int j, double p2)
        {
            for (int i = 0; i < N; i++)
            {

                if (distance[i, j] != 0)
                {
                    double pipePrice = 1000000000000;
                    foreach (var item in structureParamList)
                    {
                        p1 = getP1(p2, qleijiaArray[i], distance[i, j], item.diameter - 2 * item.thickness);
                        double totalPrice = (item.diameter - item.thickness) * item.thickness * 0.02466 * distance[i, j]*0.001 * item.price;
                        //double totalPrice = 3.1415926*(Math.Pow((item.diameter/ 2*0.001), 2)-Math.Pow(((item.diameter - 2*item.thickness)*0.001/2),2)) * distance[i, j] *7.85*0.001* item.price;
                        if ((item.standardPressure > p1) && (totalPrice <= pipePrice))
                        {
                            pipeArray[i] = structureParamList.IndexOf(item);
                            pipePrice = totalPrice;
                        }
                    }
                    pArray[i] = p1;
                    priceArray[i] = pipePrice;//存放总价
                    getPipeParam(i, p1);//计算压力公式
                }
            }

        }

        public double getP1(double p2, double q1, double L, double d)
        {
            double temp;
            double lamda = 0.009588* Math.Pow(d / 1000, -0.2);
            double Z = 0.793;
            double delt = 0.593;
            double T = 293;
            double alpha = 0;
            double h2 = 1;
            double h1 = 1;
            double delth = 1;
            double C0 = 0.03848;
            //q1 = q1 / 24 / 60 / 60;

            ////师兄比赛参数
            //delt = 0.5548;
            //Z = 0.85;
            //T = 293;



            //temp = Math.Pow(q1 / 1051.0, 2) * (lamda * Z * delt * T * L * (1 + alpha / (2 * L) * (h2 + h1) * L)) / Math.Pow(d, 5) + Math.Pow(p2, 2) * (1 + alpha * delth);
            //temp = Math.Pow(q1 / 1051.0, 2) * (xishu * L) / Math.Pow(d, 5) + Math.Pow(p2, 2) * (1 + alfa * delth);
            temp = Math.Pow(p2*1000000, 2) + lamda * Z * delt * T / (Math.Pow(C0, 2) * Math.Pow(d / 1000, 5)) * L * Math.Pow(q1 * 10000/24/3600, 2);
            //double t1 = lamda * Z * delt * T;
            //double t2= Math.Pow(C0, 2) * Math.Pow(d, 5);
            //double t3 = L * Math.Pow(q1, 2);
            //double t4 = lamda * Z * delt * T / (Math.Pow(C0, 2) * Math.Pow(d, 5)) * L * Math.Pow(q1, 2);

            ////师兄公式
            //temp = Math.Pow(p2, 2) + Z * delt * T * (L / 1000) * Math.Pow((q1 * 10000 / 5033.11 / Math.Pow(d / 10, 8 / 3)), 2);
            p1 = System.Math.Sqrt(temp)/ 1000000;//开根号
            return p1;
        }
       
    }
}
