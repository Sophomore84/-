using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 风帆电池
{
    public class UserType
    {
        public Byte SICK_CastNumber { get; set; }  //SICK扫描出铸管的数量
        public Byte SICK_CastType { get; set; }    //SICK扫描出铸管的规格
        public UInt16 SICK_CastCentreX { get; set; } //SICK扫描出铸管的中心坐标X
        public UInt16 SICK_CastCentreY { get; set; } //SICK扫描出铸管的中心坐标Y
        public UInt16 SICK_CastCentreZ { get; set; } //SICK扫描出铸管的中心坐标Z
        public Byte SICK_PoolLidHas { get; set; }  //SICK扫描池位上是否有盖子


        public UInt32 PLC_Current_X { get; set; } //PLC实际坐标X
        public UInt16 PLC_Current_Y { get; set; } //PLC实际坐标Y
        public UInt16 PLC_Current_Z { get; set; } //PLC实际坐标Z
        //public UInt16 RotateCar_Current_Angle { get; set; } //旋转小车实际角度值

        public Byte X_Arrive_Signal { get; set; } //X到位标志
        public Byte Y_Arrive_Signal { get; set; } //Y到位标志
        public Byte Z_Arrive_Signal { get; set; } //Z到位标志
        //public Byte RotateCar_Arrive_Signal { get; set; } //旋转小车到位标志

        public Byte Clamp_State { get; set; } //夹具开闭状态 开：1 闭：2
        public Byte Person_Confirm_PoolLid { get; set; } //人工确认有无盖子 有：1 无：2


        public Byte ReadBack_Write_New_Flag { get; set; } //写入PLC新执行步奏标志
        public UInt32 ReadBack_Dest_X { get; set; } //写入PLC目的坐标X
        public UInt16 ReadBack_Dest_Y { get; set; } //写入PLC目的坐标Y
        public UInt16 ReadBack_Dest_Z { get; set; } //写入PLC目的坐标Z
        //public UInt16 ReadBack_Dest_Angle { get; set; } //写入旋转小车目的角度值

        public Byte ReadBack_XEnable_Signal { get; set; } //写入PLC目的坐标X的使能信号
        public Byte ReadBack_YEnable_Signal { get; set; } //写入PLC目的坐标Y的使能信号
        public Byte ReadBack_ZEnable_Signal { get; set; } //写入PLC目的坐标Z的使能信号
        //public Byte ReadBack_AngleEnable_Signal { get; set; } //写入旋转小车目的角度值使能信号
        public Byte ReadBack_Catch_Release_Enable_Signal { get; set; }//写入抓放动作标志


        public string ReturnLib_GoodsType { get; set; }//退库，自动出库时的物料规格
        public string ReturnLib_GoodsState { get; set; }//退库，自动出库时的物料状态


        public bool threadStart_flag { get; set; }//任务线程开启标志位

    }
}
