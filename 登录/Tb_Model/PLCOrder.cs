using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 风帆电池.Tb_Model
{
    public class PlcOrder
    {
        public string OrderArea
        {
            get;
            set;
        }
        public string OrderType
        {
            get;
            set;
        }
        public string OrderAction
        {
            get;
            set;
        }
        public string ActionState
        {
            get;
            set;
        }
        public string BanZuNumber
        {
            get;
            set;
        }
        public string OrderGoodsType
        {
            get;
            set;
        }
        public string OrderGoodsState
        {
            get;
            set;
        }
        public string OrderGoodsCheckState
        {
            get;
            set;
        }
    }
}
